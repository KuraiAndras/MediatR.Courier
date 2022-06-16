namespace MediatR.Courier.Examples.Shared.Notifications;

public sealed class ExampleNotification : INotification
{
    public ExampleNotification(int notificationCount) => NotificationCount = notificationCount;

    public int NotificationCount { get; }
}
