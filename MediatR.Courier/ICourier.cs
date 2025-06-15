namespace MediatR.Courier;

/// <summary>
/// Provides functionality for subscribing to and unsubscribing from MediatR notifications
/// with support for both strong and weak reference handlers.
/// </summary>
public interface ICourier
{
    /// <summary>
    /// Subscribes a handler to receive notifications of the specified type.
    /// The handler will be held with a strong reference.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification to subscribe to.</typeparam>
    /// <param name="handler">The handler to invoke when a notification is published.</param>
    void Subscribe<TNotification>(Action<TNotification> handler)
        where TNotification : INotification;

    /// <inheritdoc cref="Subscribe{TNotification}(Action{TNotification})"/>
    /// <typeparam name="TNotification">The type of notification to subscribe to.</typeparam>
    /// <param name="handler">The handler to invoke when a notification is published.</param>
    void Subscribe<TNotification>(Action<TNotification, CancellationToken> handler)
        where TNotification : INotification;

    /// <inheritdoc cref="Subscribe{TNotification}(Action{TNotification})"/>
    /// <typeparam name="TNotification">The type of notification to subscribe to.</typeparam>
    /// <param name="handler">The handler to invoke when a notification is published.</param>
    void Subscribe<TNotification>(Func<TNotification, Task> handler)
        where TNotification : INotification;

    /// <inheritdoc cref="Subscribe{TNotification}(Action{TNotification})"/>
    /// <typeparam name="TNotification">The type of notification to subscribe to.</typeparam>
    /// <param name="handler">The handler to invoke when a notification is published.</param>
    void Subscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification;

    /// <summary>
    /// Subscribes a handler to receive notifications of the specified type using a weak reference.
    /// The handler will be held with a weak reference, allowing the target object to be garbage collected.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification to subscribe to.</typeparam>
    /// <param name="handler">The handler to invoke when a notification is published.</param>
    void SubscribeWeak<TNotification>(Action<TNotification> handler)
        where TNotification : INotification;

    /// <inheritdoc cref="SubscribeWeak{TNotification}(Action{TNotification})"/>
    /// <typeparam name="TNotification">The type of notification to subscribe to.</typeparam>
    /// <param name="handler">The handler to invoke when a notification is published.</param>
    void SubscribeWeak<TNotification>(Action<TNotification, CancellationToken> handler)
        where TNotification : INotification;

    /// <inheritdoc cref="SubscribeWeak{TNotification}(Action{TNotification})"/>
    /// <typeparam name="TNotification">The type of notification to subscribe to.</typeparam>
    /// <param name="handler">The handler to invoke when a notification is published.</param>
    void SubscribeWeak<TNotification>(Func<TNotification, Task> handler)
        where TNotification : INotification;

    /// <inheritdoc cref="SubscribeWeak{TNotification}(Action{TNotification})"/>
    /// <typeparam name="TNotification">The type of notification to subscribe to.</typeparam>
    /// <param name="handler">The handler to invoke when a notification is published.</param>
    void SubscribeWeak<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification;

    /// <summary>
    /// Unsubscribes a handler from receiving notifications of the specified type.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification to unsubscribe from.</typeparam>
    /// <param name="handler">The handler to remove from the subscription list.</param>
    void UnSubscribe<TNotification>(Action<TNotification> handler)
        where TNotification : INotification;

    /// <inheritdoc cref="UnSubscribe{TNotification}(Action{TNotification})"/>
    /// <typeparam name="TNotification">The type of notification to unsubscribe from.</typeparam>
    /// <param name="handler">The handler to remove from the subscription list.</param>
    void UnSubscribe<TNotification>(Action<TNotification, CancellationToken> handler)
        where TNotification : INotification;

    /// <inheritdoc cref="UnSubscribe{TNotification}(Action{TNotification})"/>
    /// <typeparam name="TNotification">The type of notification to unsubscribe from.</typeparam>
    /// <param name="handler">The handler to remove from the subscription list.</param>
    void UnSubscribe<TNotification>(Func<TNotification, Task> handler)
        where TNotification : INotification;

    /// <inheritdoc cref="UnSubscribe{TNotification}(Action{TNotification})"/>
    /// <typeparam name="TNotification">The type of notification to unsubscribe from.</typeparam>
    /// <param name="handler">The handler to remove from the subscription list.</param>
    void UnSubscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification;
}
