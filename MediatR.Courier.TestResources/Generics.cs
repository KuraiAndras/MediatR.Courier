namespace MediatR.Courier.TestResources;

public record GenericMessage<T>(T Data) : INotification;

public sealed class GenericMessenger
{
    public int CallCount { get; set; }
}

public class GenericHandler<T> : INotificationHandler<GenericMessage<T>>
{
    private readonly GenericMessenger _messenger;

    public GenericHandler(GenericMessenger messenger) => _messenger = messenger;

    public Task Handle(GenericMessage<T> notification, CancellationToken cancellationToken)
    {
        _messenger.CallCount++;

        return Task.CompletedTask;
    }
}

public class GenericHandler : INotificationHandler<GenericMessage<string>>
{
    private readonly GenericMessenger _messenger;

    public GenericHandler(GenericMessenger messenger) => _messenger = messenger;

    public Task Handle(GenericMessage<string> notification, CancellationToken cancellationToken)
    {
        _messenger.CallCount++;

        return Task.CompletedTask;
    }
}
