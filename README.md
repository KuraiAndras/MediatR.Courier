# MediatR.Courier [![Nuget](https://img.shields.io/nuget/v/MediatR.Courier)](https://www.nuget.org/packages/MediatR.Courier) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=KuraiAndras_MediatR.Courier&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=KuraiAndras_MediatR.Courier) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=KuraiAndras_MediatR.Courier&metric=coverage)](https://sonarcloud.io/summary/new_code?id=KuraiAndras_MediatR.Courier)

A simple library to treat MediatR notifications sort of like events.

Main usage target is client applications.

## What does this solve?

This library is aimed to provide help for client-side applications using the event aggregator pattern with MediatR.

## Usage

Install form [NuGet](https://www.nuget.org/packages/MediatR.Courier/)

Basic usage:

```c#
services
    .AddMediatR(c => c.RegisterServicesFromAssemblyContaining(typeof(MyType)))
    .AddCourier(typeof(MyType).Assembly);

ICourier _courier;

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

### Main concepts:

Events become concrete classes:

```c#
//Regular class with event
public class EventProducer
{
    public event Action ExampleEvent;
}

//Event defined when using Courier
public class ExampleEvent : INotification
{
    //Your implementation own implementation properties, methods, etc...
}

```

Working with events:

```c#
// Using an regular C# event:
void Handler()
{
    //Implementation
}
EventProducer producer.ExampleEvent += Handler;
EventProducer producer.ExampleEvent -= Handler;

// Using Courier:

void Handler(ExampleEvent notification)
{
    //Implementation
}

ICourier courier = ;//Create courier

courier.Subscribe<ExampleEvent>(Handler);
courier.UnSubscribe<ExampleEvent>(Handler);

```

Firing events

```c#
// Regular C# events
public class EventProducer
{
    public event Action ExampleEvent;

    public void RaiseEvent()
    {
        //Implementation
        ExampleEvent?.Invoke();
    }
}

// Courier
public class EventProducer
{
    private IMediator _mediator = ;//Get mediator

    public void RaiseEvent()
    {
        _mediator.Publish(new ExampleNotification());
    }
}

```

### What benefits does it provide?

* Your classes handling events don't need a direct reference to a specific instance of an event producer, or some middleman connecting them, just a reference to the Courier
* If the handler is "far" away from the event producer, you don't need to chain events through other classes to receive them

### How is this different from MediatR's INotificationHandler?

MediatR instantiates the classes implementing INotificationHandler through the provided ServiceProvider which is most of the time is some dependency injection container, thus it is rather hard to tie handling an event to a specific event handler.

Let's say you have your View which contains two windows. Both windows have the same ViewModel class. You want this ViewModel class to handle some event. What are some choices to handle the lifetime and state of those view models?

* You can make your ViewModel singleton, and when MediatR publishes the notification to the single view model instance. This creates the possible problem that your ViewModels can't hold different state in your windows.
* Have some service handle the notification, then publish it's contents as a regular C# event and then get the reference to that exact service instance to your ViewModels and subscribe to that event. This way you can easily have an individual state in multiple ViewModels, but you need some way to connect them to MediatR notifications.

Courier is a simple implementation of the second choice: Let some middleman handle notifications from MediatR and then pass those notifications to subscribers

### Isn't this pattern implemented already in some other libraries?

Yes, quite a few actually, just a few examples:
* [Prism event aggregator](https://prismlibrary.com/docs/event-aggregator.html)
* [Caliburn.Micro event aggregator](https://caliburnmicro.com/documentation/event-aggregator)
* [Zenject signals](https://github.com/modesttree/Zenject/blob/master/Documentation/Signals.md)
* [Using Rx.Net](https://github.com/shiftkey/Reactive.EventAggregator)

The difference from those implementations is that most of them implement this concept as part of their framework. When using them in Zenject or Prism you tie some of your implementation to those frameworks, thus having you depend on code you might not want to use. This library has one core dependency: MediatR, and it assumes very little about your architecture. If you are already using MediatR in your business logic layer then adding some reactiveness to it with Courier is easy.

In the same way as using MediatR can be thought of as replacing business layer services to MediatR Requests and RequestHandlers, Courier is the same for events: replacing them with INotifications.

### Weak references

You can create a weak subscription by using the `SubscribeWeak` method. This subscription uses a `WeakReference` which will let the subscriber to be garbage collected without the need to unsubscribe (although you still can unsubscribe manually). Subscribing methods on `MonoBehavior` instances in Unity3D might result in unexpected behavior, so you should be careful with it.

```c#
courier.SubscribeWeak<MyNotification>(notification => /*...*/);

courier.SubscribeWeak<MyNotification>((notification, cancellation) => /*...*/);
```

### Capturing thread context

You can configure how the Courier awaits the sent notifications. To change the behavior modify the `CaptureThreadContext` property on the `CourierOptions` class. When using dependency injection, you can change this behavior during runtime, because the `CourierOptions` is accessible through DI.

### Parallel notification handling

You can configure whether notification handlers should run sequentially or in parallel by using the `UseTaskWhenAll` property on the `CourierOptions` class:

```c#
// Configure at registration time
services.AddCourier(typeof(MyType).Assembly, options => 
{
    options.UseTaskWhenAll = true; // Enable parallel execution of handlers
});

// Or modify at runtime through dependency injection
var options = serviceProvider.GetRequiredService<CourierOptions>();
options.UseTaskWhenAll = true;
```

When `UseTaskWhenAll` is set to `true`, asynchronous notification handlers are collected and awaited concurrently using `Task.WhenAll`.

When set to `false` (the default), handlers are awaited sequentially.

## Gotchas

* No ordering is guaranteed when calling the subscribed methods
* Async void methods are not awaited.
