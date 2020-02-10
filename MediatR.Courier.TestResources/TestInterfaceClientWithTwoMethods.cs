using System.Threading;

namespace MediatR.Courier.TestResources
{
    public sealed class TestInterfaceClientWithTwoMethods : CourierInterfaceClient, ICourierNotificationHandler<TestNotification>, ICourierNotificationHandler<TestNotification2>, ICarryNotifications
    {
        public TestInterfaceClientWithTwoMethods(ICourier courier) : base(courier)
        {
        }

        public bool MessageReceived { get; private set; }
        public bool MessageReceived2 { get; private set; }

        public void Handle(TestNotification notification, CancellationToken cancellationToken = default)
        {
            MessageReceived = true;
            MessageReceivedCount++;
        }

        public void Handle(TestNotification2 notification, CancellationToken cancellationToken = default)
        {
            MessageReceived2 = true;
            MessageReceivedCount++;
        }

        public int MessageReceivedCount { get; private set; }
        public int ProperlyImplementedHandleCount => 2;
    }
}