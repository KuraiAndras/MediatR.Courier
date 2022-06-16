namespace MediatR.Courier;

public interface ICourierNotificationHandler<in TNotification>
    where TNotification : INotification
{
    void Handle(TNotification notification, CancellationToken cancellationToken = default);
}