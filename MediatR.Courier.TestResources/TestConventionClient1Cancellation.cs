using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier.TestResources
{
    public sealed class TestConventionClient1Cancellation : CourierConventionClient, ICarryNotifications
    {
        public TestConventionClient1Cancellation(ICourier courier) : base(courier)
        {
        }

        public int MessageReceivedCount { get; private set; }
        public int ProperlyImplementedHandleCount => 2;

        public void Handle(TestNotification _, CancellationToken __) => MessageReceivedCount++;

        public async Task HandleAsync(TestNotification _, CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);

            MessageReceivedCount++;
        }
    }
}