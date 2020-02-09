namespace MediatR.Courier.TestResources
{
    public sealed class TestConventionClient : CourierConventionClient
    {
        public TestConventionClient(ICourier courier) : base(courier)
        {
        }

        public bool MessageReceived { get; private set; }

        public void Handle(TestNotification _) => MessageReceived = true;
    }
}