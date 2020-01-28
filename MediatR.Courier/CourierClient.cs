using MediatR.Courier.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MediatR.Courier
{
    public abstract class CourierClient : IDisposable
    {
        private readonly ICourier _courier;
        private readonly IReadOnlyCollection<object> _actions;

        protected CourierClient(ICourier courier)
        {
            _courier = courier;
            var subType = GetType();

            _actions = subType.GetInterfaces()
                .Where(i => i == typeof(ICourierNotificationHandler<>))
                .Select(i =>
                {
                    var methodInfo = i.GetMethod(nameof(ICourierNotificationHandler<INotification>.Handle));
                    if (methodInfo is null) throw new MethodNotImplementedException($"Method Handle is not implemented from interface {nameof(INotificationHandler<INotification>)}");

                    var notificationType = i.GetGenericArguments()[0];

                    var genericActionType = typeof(Action<,>).MakeGenericType(notificationType, typeof(CancellationToken));

                    var action = Activator.CreateInstance(genericActionType);

                    var baseSubscribeMethod = _courier.GetType().GetMethod(nameof(ICourier.Subscribe));

                    if (baseSubscribeMethod is null) throw new MethodNotImplementedException($"{nameof(ICourier)} does not have a method named {nameof(ICourier.Subscribe)}");

                    var genericSubscribeMethod = baseSubscribeMethod.MakeGenericMethod(notificationType);

                    genericSubscribeMethod.Invoke(_courier, new[] { action });

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

                var baseUnSubscribeMethod = _courier.GetType().GetMethod(nameof(ICourier.UnSubscribe));

                if (baseUnSubscribeMethod is null) throw new MethodNotImplementedException();

                var genericUnSubscribeMethod = baseUnSubscribeMethod.MakeGenericMethod(notificationType);

                genericUnSubscribeMethod.Invoke(_courier, new[] { @delegate });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
