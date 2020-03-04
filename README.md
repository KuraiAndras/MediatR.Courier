# MediatR.Courier

A simple library to treat MediatR notifications sort of like events.

Main usage target is client applications.

## What does this solve?

This library is aimed to provide help for clientside applications using the event aggregator pattern.

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
Action Handler()
{
    //Implementation
}
EventProducer producer.ExampleEvent += Handler;
EventProducer producer.ExampleEvent -= Handler;

// Using Courier:

Action<ExampleNotification> Handler(ExampleEvent notification)
{
    //Implementation
}

ICourier courier = //Create courier

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
    private IMediator _mediator = //Get mediator

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

### Extras
To make subscribing and unsubscribing from events easier this library provides two helper classes to help you do your registrations:

* CourierInterfaceClient
* CourierConventionClient

If your inherit from these classes all you have to do is implement your handlers as public methods, and the base class handles the subscription of each method in the constructor, and unsubscribes them when calling Dispose.

The interface client registers implementations of the ICourierNotificationHandler, while the convention client register methods using a simple convention. It will register your methods if they:
* Return void.
* Either: have one parameter which implements INotification.
* Or: two parameters, the first implements INotification, and the second one is CancellationToken.

Any other method is not subscribed to the Courier.

Using interfaces:
```c#
public sealed class EventHandlerExample :
CourierInterfaceClient,
ICourierNotificationHandler<TestNotification>,
ICourierNotificationHandler<TestNotification2>
    {
        public EventHandlerExample(ICourier courier) : base(courier)
        {
        }

        public bool MessageReceived { get; private set; }
        public bool MessageReceived2 { get; private set; }

        public void Handle(TestNotification notification, CancellationToken cancellationToken = default)
        {
            MessageReceived = true;
        }

        public void Handle(TestNotification2 notification, CancellationToken cancellationToken = default)
        {
            MessageReceived2 = true;
        }
    }
```

Using convention:
```c#
public sealed class EventHandlerExample : CourierConventionClient
    {
        public EventHandlerExample(ICourier courier) : base(courier)
        {
        }

        // Called
        public void Handle(TestNotification _) => MessageReceivedCount++;
        // Called
        public void Handle(TestNotification _, CancellationToken __) => MessageReceivedCount++;
        // Called
        public void HandleOptional(TestNotification _ = default) => MessageReceivedCount++;
        // Called
        public void HandleOptional2(TestNotification _, CancellationToken __ = default) => MessageReceivedCount++;
        // Called
        public void HandleOptional3(TestNotification _ = default, CancellationToken __ = default) => MessageReceivedCount++;
        // Not Called
        public void Handle(TestNotification _, CancellationToken __, TestConventionClient1Cancellation ___) => MessageReceivedCount++;
        // Not Called
        public int HandleReturnsInt(TestNotification _) => MessageReceivedCount++;

        // Called, Handlers which return Task are awaited
        public async Task HandleAsync(TestNotification _)
        {
            await Task.Delay(100);
            MessageReceivedCOunt++;
        }

        // Called, Handlers which return Task are awaited
        public Task HandleAsync(TestNotification _, CancellationToken __)
        {
            await Task.Delay(100);
            MessageReceivedCOunt++;
        }
    }
```
## Gotchas

* No ordering is guaranteed when calling the subscribed methods
* Async void methods are not awaited.

## Usage

Install form [nuget](https://www.nuget.org/packages/MediatR.Courier/)
Microsoft.Extensions.DependencyInjection helper on [nuget](https://www.nuget.org/packages/MediatR.Courier.DependencyInjection/)

Basic usage:

```c#
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

To register Courier to MediatR the simplest solution is using dependency injection and registering Courier with the provided dependency injection package