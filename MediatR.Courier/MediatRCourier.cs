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

        public void Subscribe<TNotification>(Action<TNotification> action) where TNotification : INotification =>
            SubscribeInternal<TNotification>(action, false);

        public void Subscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification =>
            SubscribeInternal<TNotification>(action, true);

        private void SubscribeInternal<TNotification>(Delegate @delegate, bool needsCancellation) where TNotification : INotification
        {
            var notificationType = typeof(TNotification);
            if (_actions.TryGetValue(notificationType, out var subscribers))
            {
                subscribers.Add((@delegate, needsCancellation));
            }
            else
            {
                _actions.TryAdd(notificationType, new ConcurrentBag<(Delegate, bool)>(new ValueTuple<Delegate, bool>[] { (@delegate, needsCancellation) }));
            }
        }

        public void UnSubscribe<TNotification>(Action<TNotification> action) where TNotification : INotification => UnSubscribeInternal<TNotification>(action);

        public void UnSubscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification => UnSubscribeInternal<TNotification>(action);

        private void UnSubscribeInternal<TNotification>(Delegate @delegate) where TNotification : INotification
        {
            var notificationType = typeof(TNotification);
            if (!_actions.TryGetValue(notificationType, out var subscribers)) return;

            var remainingSubscribers = new ConcurrentBag<(Delegate, bool)>(subscribers.Where(subscriber => !subscriber.action.Equals(@delegate)));

            _actions.TryRemove(notificationType, out _);
            _actions.TryAdd(notificationType, remainingSubscribers);
        }
    }
}