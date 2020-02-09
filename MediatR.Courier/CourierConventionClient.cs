using MediatR.Courier.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MediatR.Courier
{
    public abstract class CourierConventionClient : IDisposable
    {
        private readonly ICourier _courier;
        private readonly ICollection<Delegate> _actions;

        protected CourierConventionClient(ICourier courier)
        {
            _courier = courier;

            var subType = GetType();

            var baseNotificationType = typeof(INotification);
            var cancellationTokenType = typeof(CancellationToken);

            _actions = subType.GetMethods()
                .Select(method =>
                {
                    var parameters = method.GetParameters();

                    if (parameters.Length > 2 || parameters.Length == 0) return null;

                    var notificationType = parameters[0].ParameterType;
                    if (!baseNotificationType.IsAssignableFrom(notificationType)) return null;

                    var (actionType, hasCancellation) = parameters.Length == 1
                        ? (typeof(Action<>).MakeGenericType(notificationType), false)
                        : (typeof(Action<,>).MakeGenericType(notificationType, cancellationTokenType), true);

                    var @delegate = Delegate.CreateDelegate(actionType, this, method);

                    var subscribeMethod = _courier.GetCourierMethod(nameof(ICourier.Subscribe), hasCancellation, notificationType);

                    subscribeMethod.Invoke(_courier, new object[] { @delegate });

                    return @delegate;
                })
                .Where(m => !(m is null))
                .ToList();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            foreach (var @delegate in _actions)
            {
                var parameters = @delegate.GetMethodInfo().GetParameters();

                var notificationType = parameters[0].ParameterType;

                var hasCancellation = parameters.Length != 1;

                var unSubscribeMethod = _courier.GetCourierMethod(nameof(ICourier.UnSubscribe), hasCancellation, notificationType);

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