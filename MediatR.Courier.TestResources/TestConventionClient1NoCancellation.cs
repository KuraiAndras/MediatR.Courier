namespace MediatR.Courier.TestResources
{
    public sealed class TestConventionClient1NoCancellation : CourierConventionClient, ICarryNotifications
    {
        public TestConventionClient1NoCancellation(ICourier courier) : base(courier)
        {
        }

        public void Handle(TestNotification _) => MessageReceivedCount++;
        public int MessageReceivedCount { get; private set; }
        public int ProperlyImplementedHandleCount => 1;
    }
}