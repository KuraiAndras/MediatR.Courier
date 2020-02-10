namespace MediatR.Courier.TestResources
{
    public sealed class TestConventionClient1Cancellation : CourierConventionClient, ICarryNotifications
    {
        public TestConventionClient1Cancellation(ICourier courier) : base(courier)
        {
        }

        public int MessageReceivedCount { get; private set; }
        public int ProperlyImplementedHandleCount => 1;

        public void Handle(TestNotification _) => MessageReceivedCount++;
    }
}