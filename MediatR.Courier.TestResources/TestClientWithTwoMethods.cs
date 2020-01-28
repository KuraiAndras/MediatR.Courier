using System.Threading;

namespace MediatR.Courier.TestResources
{
    public sealed class TestClientWithTwoMethods : CourierClient, ICourierNotificationHandler<TestNotification>, ICourierNotificationHandler<TestNotification2>
    {
        public TestClientWithTwoMethods(ICourier courier) : base(courier)
        {
        }

        public bool MessageReceived { get; private set; }
        public bool MessageReceived2 { get; private set; }

        public void Handle(TestNotification notification, CancellationToken cancellationToken = default) => MessageReceived = true;

        public void Handle(TestNotification2 notification, CancellationToken cancellationToken = default) => MessageReceived2 = true;
    }
}