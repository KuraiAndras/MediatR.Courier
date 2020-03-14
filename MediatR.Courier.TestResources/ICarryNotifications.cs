namespace MediatR.Courier.TestResources
{
    public interface ICarryNotifications
    {
        int MessageReceivedCount { get; }

        int ProperlyImplementedHandleCount { get; }
    }
}