using System.Threading;

namespace MediatR.Courier.TestResources
{
    public sealed class TestInterfaceClient1Cancellation : CourierInterfaceClient, ICourierNotificationHandler<TestNotification>, ICarryNotifications
    {
        public TestInterfaceClient1Cancellation(ICourier courier)
            : base(courier)
        {
        }

        public bool MessageReceived { get; private set; }

        public int MessageReceivedCount { get; private set; }

        public int ProperlyImplementedHandleCount => 1;

        public void Handle(TestNotification notification, CancellationToken cancellationToken = default)
        {
            MessageReceived = true;
            MessageReceivedCount++;
        }
    }
}
