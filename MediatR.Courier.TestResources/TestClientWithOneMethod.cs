using System.Threading;

namespace MediatR.Courier.TestResources
{
    public sealed class TestClientWithOneMethod : CourierClient, ICourierNotificationHandler<TestNotification>
    {
        public TestClientWithOneMethod(ICourier courier) : base(courier)
        {
        }

        public bool MessageReceived { get; private set; }

        public void Handle(TestNotification notification, CancellationToken cancellationToken = default) => MessageReceived = true;
    }
}
