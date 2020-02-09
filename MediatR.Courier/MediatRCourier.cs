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
            Subscribe<TNotification>(action, false);

        public void Subscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification =>
            Subscribe<TNotification>(action, true);

        private void Subscribe<TNotification>(Delegate action, bool needsCancellation) where TNotification : INotification
        {
            var notificationType = typeof(TNotification);
            if (_actions.TryGetValue(notificationType, out var subscribers))
            {
                subscribers.Add((action, needsCancellation));
            }
            else
            {
                _actions.TryAdd(notificationType, new ConcurrentBag<(Delegate, bool)>(new ValueTuple<Delegate, bool>[] { (action, needsCancellation) }));
            }
        }

        public void UnSubscribe<TNotification>(Action<TNotification> action) where TNotification : INotification => UnSubscribe<TNotification>((Delegate) action);

        public void UnSubscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification => UnSubscribe<TNotification>((Delegate) action);

        private void UnSubscribe<TNotification>(Delegate @delegate) where TNotification : INotification
        {
            var notificationType = typeof(TNotification);
            if (!_actions.TryGetValue(notificationType, out var subscribers)) return;

            var remainingSubscribers = new ConcurrentBag<(Delegate, bool)>(subscribers.Where(subscriber => !subscriber.action.Equals(@delegate)));

            _actions.TryRemove(notificationType, out _);
            _actions.TryAdd(notificationType, remainingSubscribers);
        }
    }
}