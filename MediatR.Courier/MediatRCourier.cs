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
            Subscribe<TNotification>((action, false));

        public void Subscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification =>
            Subscribe<TNotification>((action, true));

        public void Subscribe<TNotification>(Func<TNotification, Task> action) where TNotification : INotification => throw new NotImplementedException();

        public void Subscribe<TNotification>(Func<TNotification, CancellationToken, Task> action) where TNotification : INotification => throw new NotImplementedException();

        private void Subscribe<TNotification>(ValueTuple<Delegate, bool> subscriber) where TNotification : INotification
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

        public void UnSubscribe<TNotification>(Action<TNotification> action) where TNotification : INotification => UnSubscribe<TNotification>((Delegate)action);

        public void UnSubscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification => UnSubscribe<TNotification>((Delegate)action);

        public void UnSubscribe<TNotification>(Func<TNotification, Task> action) where TNotification : INotification => throw new NotImplementedException();

        public void UnSubscribe<TNotification>(Func<TNotification, CancellationToken, Task> action) where TNotification : INotification => throw new NotImplementedException();

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