using System;
using System.Threading.Tasks;

namespace MediatR.Courier.TestResources
{
    public sealed class TestConventionClient1NoCancellation : CourierConventionClient, ICarryNotifications
    {
        public TestConventionClient1NoCancellation(ICourier courier) : base(courier)
        {
        }

        public void Handle(TestNotification _) => MessageReceivedCount++;

        public async Task HandleAsync(TestNotification _)
        {
            await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);

            MessageReceivedCount++;
        }

        public int MessageReceivedCount { get; private set; }
        public int ProperlyImplementedHandleCount => 2;
    }
}