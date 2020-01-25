# MediatR.Courier

A simple library to treat MediatR notifications sort of like events.

Main usage target is client applications.

## Usage

Install form [nuget]()

```c#
private readonly ICourier _courier;

void SubscribeToCourier()
{
    // Subscribe to a specific notification type your want to receive.
    _courier.Subscribe<ExampleNotification>(HandleNotification)
}

void HandleNotification(ExampleNotification notification, CancellationToken cancellationToken)
{
    //Do your handling logic here.
    Console.WriteLine("ExampleNotification handled");
}

void UnsubscribeFromCourier()
{
    // Unsubscribe with the same delegate you subscribed with, just like with events.
    _courier.UnSubscribe(HandleNotification);
}

// Somewhere else.

private readonly IMediator _mediator;

async Task FireNotification()
{
    // Somewhere some class publishes a notification with the mediator.
    // Courier is just a specialized INotificationHandler<INotification> implementation.
    await _mediator.Publish(new ExampleNotification());
}
```

To register Courier to MediatR the simplest solution is using dependency injection and registering Courier with the provided dependency injection package

A complete example usage in a WPF MVVM application can be found in the MediatR.Courier.Example.Wpf.Core project