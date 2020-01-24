using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier
{
    public sealed class MediatRCourier<TNotificationType> : ICourier, INotificationHandler<TNotificationType> where TNotificationType : INotification
    {
        private readonly ConcurrentDictionary<Type, ConcurrentBag<object>> _actions = new ConcurrentDictionary<Type, ConcurrentBag<object>>();

        public bool TrySubscribe<TNotification>(Action<TNotification> action) where TNotification : INotification
        {
            var notificationType = typeof(TNotification);
            if (!_actions.TryGetValue(notificationType, out var subscribers))
            {
                return _actions.TryAdd(notificationType, new ConcurrentBag<object>(new[] { action }));
            }

            subscribers.Add(action);

            return true;
        }

        public void UnSubscribe<TNotification>(Action<TNotification> action) where TNotification : INotification
        {
            var notificationType = typeof(TNotification);
            if (!_actions.TryGetValue(notificationType, out var subscribers)) return;

            var newSubscribers = new ConcurrentBag<object>(subscribers.Where(subscriber => !subscriber.Equals(action)));

            _actions.TryRemove(notificationType, out _);
            _actions.TryAdd(notificationType, newSubscribers);
        }

        public Task Handle(TNotificationType notification, CancellationToken cancellationToken)
        {
            var notificationType = notification.GetType();
            var completedTask = Task.CompletedTask;
            if (!_actions.TryGetValue(notificationType, out var subscribers)) return completedTask;

            foreach (var subscriber in subscribers)
            {
                ((Action<TNotificationType>)subscriber).Invoke(notification);
            }

            return completedTask;
        }
    }
}