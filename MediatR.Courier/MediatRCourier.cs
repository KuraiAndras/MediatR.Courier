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

        public async Task Handle(INotification notification, CancellationToken cancellationToken)
        {
            var notificationType = notification.GetType();

            if (!_actions.TryGetValue(notificationType, out var subscribers)) return;

            foreach (var (action, needsToken) in subscribers)
            {
                var parameters = needsToken
                    ? new object[] { notification, cancellationToken }
                    : new object[] { notification };

                var result = action.DynamicInvoke(parameters);
                if (result is Task task) await task.ConfigureAwait(false);
            }
        }

        public void Subscribe<TNotification>(Action<TNotification> handler) where TNotification : INotification =>
            Subscribe<TNotification>((handler, false));

        public void Subscribe<TNotification>(Action<TNotification, CancellationToken> handler) where TNotification : INotification =>
            Subscribe<TNotification>((handler, true));

        public void Subscribe<TNotification>(Func<TNotification, Task> handler) where TNotification : INotification =>
            Subscribe<TNotification>((handler, false));

        public void Subscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler) where TNotification : INotification =>
            Subscribe<TNotification>((handler, true));

        private void Subscribe<TNotification>((Delegate handler, bool needsCancellation) subscriber) where TNotification : INotification
        {
            var notificationType = typeof(TNotification);
            if (_actions.TryGetValue(notificationType, out var subscribers))
            {
                subscribers.Add(subscriber);
            }
            else
            {
                _actions.TryAdd(notificationType, new ConcurrentBag<(Delegate, bool)>(new[] { subscriber }));
            }
        }

        public void UnSubscribe<TNotification>(Action<TNotification> handler) where TNotification : INotification =>
            UnSubscribe<TNotification>((Delegate)handler);

        public void UnSubscribe<TNotification>(Action<TNotification, CancellationToken> handler) where TNotification : INotification =>
            UnSubscribe<TNotification>((Delegate)handler);

        public void UnSubscribe<TNotification>(Func<TNotification, Task> handler) where TNotification : INotification =>
            UnSubscribe<TNotification>((Delegate)handler);

        public void UnSubscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler) where TNotification : INotification =>
            UnSubscribe<TNotification>((Delegate)handler);

        private void UnSubscribe<TNotification>(Delegate handler) where TNotification : INotification
        {
            var notificationType = typeof(TNotification);
            if (!_actions.TryGetValue(notificationType, out var subscribers)) return;

            var remainingSubscribers = new ConcurrentBag<(Delegate, bool)>(subscribers.Where(subscriber => !subscriber.action.Equals(handler)));

            _actions.TryRemove(notificationType, out _);
            _actions.TryAdd(notificationType, remainingSubscribers);
        }
    }
}