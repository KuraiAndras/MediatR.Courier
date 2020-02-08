using MediatR.Courier.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MediatR.Courier.Extensions;

namespace MediatR.Courier
{
    public abstract class CourierInterfaceClient : IDisposable
    {
        private readonly ICourier _courier;
        private readonly IReadOnlyCollection<Delegate> _actions;

        protected CourierInterfaceClient(ICourier courier)
        {
            _courier = courier;
            var subType = GetType();

            _actions = subType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICourierNotificationHandler<>))
                .Select(i =>
                {
                    var notificationHandleMethodInfo = i.GetMethod(nameof(ICourierNotificationHandler<INotification>.Handle));
                    if (notificationHandleMethodInfo is null) throw new MethodNotImplementedException($"Method Handle is not implemented from interface {nameof(INotificationHandler<INotification>)}");

                    var notificationType = i.GetGenericArguments()[0];

                    var genericActionType = typeof(Action<,>).MakeGenericType(notificationType, typeof(CancellationToken));

                    var action = Delegate.CreateDelegate(genericActionType, this, notificationHandleMethodInfo);

                    var baseSubscribeMethod = _courier.GetCourierMethod(CourierMethodName.Subscribe, CourierMethodType.Cancellation);

                    if (baseSubscribeMethod is null) throw new MethodNotImplementedException($"{nameof(ICourier)} does not have a method named {nameof(ICourier.Subscribe)}");

                    var genericSubscribeMethod = baseSubscribeMethod.MakeGenericMethod(notificationType);

                    genericSubscribeMethod.Invoke(_courier, new object[] { action });

                    return action;
                })
                .ToList()
                .AsReadOnly();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            foreach (var @delegate in _actions)
            {
                var notificationType = @delegate.GetType().GetGenericArguments()[0];

                var baseUnSubscribeMethod = _courier.GetCourierMethod(CourierMethodName.UnSubscribe, CourierMethodType.Cancellation);

                if (baseUnSubscribeMethod is null) throw new MethodNotImplementedException();

                var genericUnSubscribeMethod = baseUnSubscribeMethod.MakeGenericMethod(notificationType);

                genericUnSubscribeMethod.Invoke(_courier, new object[] { @delegate });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
