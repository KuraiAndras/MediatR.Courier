using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier
{
    public sealed class MediatRCourier : ICourier, INotificationHandler<INotification>
    {
        private readonly ConcurrentDictionary<Type, ConcurrentBag<(Delegate action, bool needsToken)>> _actions = new ConcurrentDictionary<Type, ConcurrentBag<(Delegate, bool)>>();

        public Task Handle(INotification notification, CancellationToken cancellationToken)
        {
            var notificationType = notification.GetType();
            var completedTask = Task.CompletedTask;
            if (!_actions.TryGetValue(notificationType, out var subscribers)) return completedTask;

            foreach (var (action, needsToken) in subscribers)
            {
                if (needsToken)
                {
                    action.DynamicInvoke(notification, cancellationToken);
                }
                else
                {
                    action.DynamicInvoke(notification);
                }
            }

            return completedTask;
        }

        public void Subscribe<TNotification>(Action<TNotification> action) where TNotification : INotification => throw new NotImplementedException();

        public void Subscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification
        {
            var notificationType = typeof(TNotification);
            if (_actions.TryGetValue(notificationType, out var subscribers))
            {
                subscribers.Add((action, true));
            }
            else
            {
                _actions.TryAdd(notificationType, new ConcurrentBag<(Delegate, bool)>(new ValueTuple<Delegate, bool>[] { (action, true) }));
            }
        }

        public void UnSubscribe<TNotification>(Action<TNotification> action) where TNotification : INotification => throw new NotImplementedException();

        public void UnSubscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification
        {
            var notificationType = typeof(TNotification);
            if (!_actions.TryGetValue(notificationType, out var subscribers)) return;

            var remainingSubscribers = new ConcurrentBag<(Delegate, bool)>(subscribers.Where(subscriber => !subscriber.action.Equals(action)));

            _actions.TryRemove(notificationType, out _);
            _actions.TryAdd(notificationType, remainingSubscribers);
        }
    }
}