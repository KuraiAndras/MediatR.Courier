﻿using System;
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

            foreach (var subscriber in subscribers)
            {
                var genericActionType = typeof(Action<>).MakeGenericType(notificationType);
                var invokeMethod = genericActionType.GetMethod("Invoke");
                invokeMethod?.Invoke(subscriber, new object[] { notification });
            }

            return completedTask;
        }

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
    }
}