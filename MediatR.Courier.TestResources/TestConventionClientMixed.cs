using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier.TestResources
{
#pragma warning disable IDE0060 // Remove unused parameter
    public sealed class TestConventionClientMixed : CourierConventionClient, ICarryNotifications
    {
        public TestConventionClientMixed(ICourier courier)
            : base(courier)
        {
        }

        public int MessageReceivedCount { get; private set; }

        public int ProperlyImplementedHandleCount => 7;

        public void Handle(TestNotification _) => MessageReceivedCount++;

        public void Handle(TestNotification _, CancellationToken __) => MessageReceivedCount++;

        public void Handle(TestNotification _, CancellationToken __, TestConventionClient1Cancellation ___) => MessageReceivedCount++;

        public void HandleOptional(TestNotification? _ = default) => MessageReceivedCount++;

        public void HandleOptional2(TestNotification _, CancellationToken __ = default) => MessageReceivedCount++;

        public void HandleOptional3(TestNotification? _ = default, CancellationToken __ = default) => MessageReceivedCount++;

        public int HandleReturnsInt(TestNotification _) => MessageReceivedCount++;

        public async Task HandleAsync(TestNotification _)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1)).ConfigureAwait(false);
            MessageReceivedCount++;
        }

        public async Task HandleAsync(TestNotification _, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);
            MessageReceivedCount++;
        }
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
