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

                    switch (parameters.Length)
                    {
                        case 1:
                            {
                                var genericActionType = typeof(Action<>).MakeGenericType(notificationType);

                                var action = Delegate.CreateDelegate(genericActionType, this, method);

                                var subscribeMethod = _courier.GetCourierMethod(CourierMethodName.Subscribe, CourierMethodType.NoCancellation, notificationType);

                                subscribeMethod.Invoke(_courier, new object[] { action });

                                return action;
                            }
                        case 2:
                            {
                                var genericActionType = typeof(Action<,>).MakeGenericType(notificationType, cancellationTokenType);

                                var action = Delegate.CreateDelegate(genericActionType, this, method);

                                var subscribeMethod = _courier.GetCourierMethod(CourierMethodName.Subscribe, CourierMethodType.Cancellation, notificationType);

                                subscribeMethod.Invoke(_courier, new object[] { action });

                                return action;
                            }
                        default:
                            return null;
                    }
                })
                .Where(m => !(m is null))
                .ToList();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            var baseNotificationType = typeof(INotification);
            var cancellationTokenType = typeof(CancellationToken);
            foreach (var @delegate in _actions)
            {
                var method = @delegate.GetMethodInfo();

                var parameters = method.GetParameters();

                var notificationType = parameters[0].ParameterType;
                if (!baseNotificationType.IsAssignableFrom(notificationType)) return;

                switch (parameters.Length)
                {
                    case 1:
                        {
                            var genericActionType = typeof(Action<>).MakeGenericType(notificationType);

                            var action = Delegate.CreateDelegate(genericActionType, this, method);

                            var subscribeMethod = _courier.GetCourierMethod(CourierMethodName.UnSubscribe, CourierMethodType.NoCancellation, notificationType);

                            subscribeMethod.Invoke(_courier, new object[] { action });

                            return;
                        }
                    case 2:
                        {
                            var genericActionType = typeof(Action<,>).MakeGenericType(notificationType, cancellationTokenType);

                            var action = Delegate.CreateDelegate(genericActionType, this, method);

                            var subscribeMethod = _courier.GetCourierMethod(CourierMethodName.UnSubscribe, CourierMethodType.Cancellation, notificationType);

                            subscribeMethod.Invoke(_courier, new object[] { action });

                            return;
                        }
                }
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
