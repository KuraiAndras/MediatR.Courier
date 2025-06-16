namespace MediatR.Courier.Tests.TestResources;

public interface ICarryNotifications
{
    int MessageReceivedCount { get; }

    int ProperlyImplementedHandleCount { get; }
}
