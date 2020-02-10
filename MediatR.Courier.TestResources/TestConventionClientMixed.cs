using System.Threading;

namespace MediatR.Courier.TestResources
{
    public sealed class TestConventionClientMixed : CourierConventionClient, ICarryNotifications
    {
        public TestConventionClientMixed(ICourier courier) : base(courier)
        {
        }

        public int MessageReceivedCount { get; private set; }
        public int ProperlyImplementedHandleCount => 2;

        public void Handle(TestNotification _) => MessageReceivedCount++;
        public void Handle(TestNotification _, CancellationToken __) => MessageReceivedCount++;
        public void Handle(TestNotification _, CancellationToken __, TestConventionClient1Cancellation ___) => MessageReceivedCount++;
        public int HandleReturnsInt(TestNotification _) => MessageReceivedCount++;
    }
}