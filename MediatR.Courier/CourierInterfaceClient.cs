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
        private readonly ICollection<Delegate> _actions;

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

                    var subscribeMethod = _courier.GetCourierMethod(nameof(ICourier.Subscribe), true, notificationType);

                    subscribeMethod.Invoke(_courier, new object[] { action });

                    return action;
                })
                .ToList();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            foreach (var @delegate in _actions)
            {
                var notificationType = @delegate.GetType().GetGenericArguments()[0];

                var unSubscribeMethod = _courier.GetCourierMethod(nameof(ICourier.UnSubscribe), true, notificationType);

                unSubscribeMethod.Invoke(_courier, new object[] { @delegate });
            }

            _actions.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
