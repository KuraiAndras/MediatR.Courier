using System.Threading;

namespace MediatR.Courier.TestResources
{
    public sealed class TestInterfaceClient1Cancellation : CourierInterfaceClient, ICourierNotificationHandler<TestNotification>, ICarryNotifications
    {
        public TestInterfaceClient1Cancellation(ICourier courier) : base(courier)
        {
        }

        public bool MessageReceived { get; private set; }

        public void Handle(TestNotification notification, CancellationToken cancellationToken = default)
        {
            MessageReceived = true;
            MessageReceivedCount++;
        }

        public int MessageReceivedCount { get; private set; }
        public int ProperlyImplementedHandleCount => 1;
    }

    public sealed class TestConventionClient3Mixed : CourierConventionClient, ICarryNotifications
    {
        public TestConventionClient3Mixed(ICourier courier) : base(courier)
        {
        }

        public int MessageReceivedCount { get; private set; }
        public int ProperlyImplementedHandleCount => 2;

        public void Handle(TestNotification _) => MessageReceivedCount++;
        public void Handle(TestNotification _, CancellationToken __) => MessageReceivedCount++;
        public void Handle(TestNotification _, CancellationToken __, TestConventionClient1Cancellation ___) => MessageReceivedCount++;
    }
}
