namespace MediatR.Courier.Examples.Wpf.Core.Notifications;

public sealed class ExampleNotification : INotification
{
    public ExampleNotification(int notificationCount) => NotificationCount = notificationCount;

    public int NotificationCount { get; }
}
