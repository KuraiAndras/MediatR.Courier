using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier
{
    public sealed class MediatRCourier : ICourier, INotificationHandler<INotification>
    {
        private readonly ConcurrentDictionary<Type, ConcurrentBag<object>> _actions = new ConcurrentDictionary<Type, ConcurrentBag<object>>();

        public Task Handle(INotification notification, CancellationToken cancellationToken)
        {
            var notificationType = notification.GetType();
            var completedTask = Task.CompletedTask;
            if (!_actions.TryGetValue(notificationType, out var subscribers)) return completedTask;

            var cancellationTokenType = typeof(CancellationToken);
            foreach (var subscriber in subscribers)
            {
                var genericActionType = typeof(Action<,>).MakeGenericType(notificationType, cancellationTokenType);
                var invokeMethod = genericActionType.GetMethod(nameof(Action<object>.Invoke));
                invokeMethod?.Invoke(subscriber, new object[] { notification, cancellationToken });
            }

            return completedTask;
        }

        public void Subscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification
        {
            var notificationType = typeof(TNotification);
            if (_actions.TryGetValue(notificationType, out var subscribers))
            {
                subscribers.Add(action);
            }
            else
            {
                _actions.TryAdd(notificationType, new ConcurrentBag<object>(new[] { action }));
            }
        }

        public void UnSubscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification
        {
            var notificationType = typeof(TNotification);
            if (!_actions.TryGetValue(notificationType, out var subscribers)) return;

            var remainingSubscribers = new ConcurrentBag<object>(subscribers.Where(subscriber => !subscriber.Equals(action)));

            _actions.TryRemove(notificationType, out _);
            _actions.TryAdd(notificationType, remainingSubscribers);
        }
    }
}