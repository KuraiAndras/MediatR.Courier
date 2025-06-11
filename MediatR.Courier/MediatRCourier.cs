using System.Collections.Concurrent;
using System.Reflection;

namespace MediatR.Courier;

public sealed class MediatRCourier : ICourier, INotificationHandler<INotification>
{
    private readonly ConcurrentDictionary<Type, ConcurrentBag<(Delegate action, bool needsToken)>> _actions = new();
    private readonly ConcurrentDictionary<Type, ConcurrentBag<(WeakReference<object> target, MethodInfo methodInfo, bool needsToken)>> _weakActions = new();

    private readonly CourierOptions _options;

    public MediatRCourier(CourierOptions options) => _options = options;

    public Task Handle(INotification notification, CancellationToken cancellationToken)
    {
        async Task HandleLocal(INotification n, CancellationToken c)
        {
            var notificationType = n.GetType();

            if (!_actions.TryGetValue(notificationType, out var subscribers)) subscribers = new();
            if (!_weakActions.TryGetValue(notificationType, out var weakSubscribers)) weakSubscribers = new();

            var remainingSubscribers = new ConcurrentBag<(WeakReference<object> target, MethodInfo methodInfo, bool needsToken)>();

            foreach (var (target, methodInfo, needsToken) in weakSubscribers)
            {
                if (target.TryGetTarget(out var handler))
                {
                    var parameters = needsToken
                        ? new object[] { n, c }
                        : new object[] { n };

                    var result = methodInfo.Invoke(handler, parameters);
                    if (result is Task task) await task.ConfigureAwait(_options.CaptureThreadContext);

                    remainingSubscribers.Add((target, methodInfo, needsToken));
                }
            }

            _weakActions.TryRemove(notificationType, out _);
            _weakActions.TryAdd(notificationType, remainingSubscribers);

            foreach (var (action, needsToken) in subscribers)
            {
                var parameters = needsToken
                    ? new object[] { n, c }
                    : new object[] { n };

                var result = action.Method.Invoke(action.Target, parameters);
                if (result is Task task) await task.ConfigureAwait(_options.CaptureThreadContext);
            }
        }

        return notification is null ? throw new ArgumentNullException(nameof(notification)) : HandleLocal(notification, cancellationToken);
    }

    public void Subscribe<TNotification>(Action<TNotification> handler)
        where TNotification : INotification =>
        Subscribe<TNotification>((handler, false), false);

    public void Subscribe<TNotification>(Action<TNotification, CancellationToken> handler)
        where TNotification : INotification =>
        Subscribe<TNotification>((handler, true), false);

    public void Subscribe<TNotification>(Func<TNotification, Task> handler)
        where TNotification : INotification =>
        Subscribe<TNotification>((handler, false), false);

    public void Subscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification =>
        Subscribe<TNotification>((handler, true), false);

    public void SubscribeWeak<TNotification>(Action<TNotification> handler)
        where TNotification : INotification =>
        Subscribe<TNotification>((handler, false), true);

    public void SubscribeWeak<TNotification>(Action<TNotification, CancellationToken> handler)
        where TNotification : INotification =>
        Subscribe<TNotification>((handler, true), true);

    public void SubscribeWeak<TNotification>(Func<TNotification, Task> handler)
        where TNotification : INotification =>
        Subscribe<TNotification>((handler, false), true);

    public void SubscribeWeak<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification =>
        Subscribe<TNotification>((handler, true), true);

    public void UnSubscribe<TNotification>(Action<TNotification> handler)
        where TNotification : INotification =>
        UnSubscribe<TNotification>((Delegate)handler);

    public void UnSubscribe<TNotification>(Action<TNotification, CancellationToken> handler)
        where TNotification : INotification =>
        UnSubscribe<TNotification>((Delegate)handler);

    public void UnSubscribe<TNotification>(Func<TNotification, Task> handler)
        where TNotification : INotification =>
        UnSubscribe<TNotification>((Delegate)handler);

    public void UnSubscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification =>
        UnSubscribe<TNotification>((Delegate)handler);

    private void Subscribe<TNotification>((Delegate handler, bool needsCancellation) subscriber, bool weak)
        where TNotification : INotification
    {
        var notificationType = typeof(TNotification);

        if (weak)
        {
            var weakSubscriber = (new WeakReference<object>(subscriber.handler.Target!), subscriber.handler.Method, subscriber.needsCancellation);

            if (_weakActions.TryGetValue(notificationType, out var subscribers))
            {
                subscribers.Add(weakSubscriber);
            }
            else
            {
                _weakActions.TryAdd(notificationType, new ConcurrentBag<(WeakReference<object> target, MethodInfo methodInfo, bool needsToken)>(new[] { weakSubscriber }));
            }
        }
        else
        {
            if (_actions.TryGetValue(notificationType, out var subscribers))
            {
                subscribers.Add(subscriber);
            }
            else
            {
                _actions.TryAdd(notificationType, new ConcurrentBag<(Delegate, bool)>(new[] { subscriber }));
            }
        }
    }

    private void UnSubscribe<TNotification>(Delegate handler)
        where TNotification : INotification
    {
        var notificationType = typeof(TNotification);
        Remove(handler, notificationType);
        RemoveWeak(handler, notificationType);
    }

    private void Remove(Delegate handler, Type notificationType)
    {
        if (!_actions.TryGetValue(notificationType, out var subscribers)) return;

        var remainingSubscribers = new ConcurrentBag<(Delegate, bool)>(subscribers.Where(subscriber => subscriber.action != handler));

        _actions.TryRemove(notificationType, out _);
        _actions.TryAdd(notificationType, remainingSubscribers);
    }

    private void RemoveWeak(Delegate handler, Type notificationType)
    {
        if (!_weakActions.TryGetValue(notificationType, out var subscribers)) return;

        var remainingSubscribers = new ConcurrentBag<(WeakReference<object> target, MethodInfo methodInfo, bool needsToken)>
        (
            subscribers.Where(subscriber => !subscriber.target.TryGetTarget(out var weakHandler) && weakHandler != handler.Target)
        );

        _weakActions.TryRemove(notificationType, out _);
        _weakActions.TryAdd(notificationType, remainingSubscribers);
    }
}