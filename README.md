# MediatR.Courier

A simple library to treat MediatR notifications sort of like events.

Main usage target is client applications.

## Usage

Install form [nuget]()

```c#
private readonly ICourier _courier;

void SubscribeToCourier()
{
    _courier.Subscribe<ExampleNotification>(HandleNotification)
}

void HandleNotification(ExampleNotification notification, CancellationToken cancellationToken)
{
    //Do your handling logic here
    Console.WriteLine("ExampleNotification handled");
}

void UnsubscribeFromCourier()
{
    _courier.UnSubscribe(HandleNotification);
}
```

A complete example usage in a WPF MVVM application can be found [here](../blob/master/MediatR.Courier.Examples.Wpf.Core)