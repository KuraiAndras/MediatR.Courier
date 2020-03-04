using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier.TestResources
{
    public sealed class TestInterfaceClientWithTwoAsyncMethods :
        CourierInterfaceClient,
        ICourierNotificationHandlerAsync<TestNotification>,
        ICourierNotificationHandlerAsync<TestNotification2>,
        ICarryNotifications
    {
        public bool MessageReceived { get; private set; }
        public bool MessageReceived2 { get; private set; }

        public TestInterfaceClientWithTwoAsyncMethods(ICourier courier) : base(courier)
        {
        }

        public async Task HandleAsync(TestNotification notification, CancellationToken cancellationToken = default)
        {
            MessageReceived = true;
            MessageReceivedCount++;

            await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);
        }

        public async Task HandleAsync(TestNotification2 notification, CancellationToken cancellationToken = default)
        {
            MessageReceived2 = true;
            MessageReceivedCount++;

            await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);
        }

        public int MessageReceivedCount { get; private set; }
        public int ProperlyImplementedHandleCount => 2;
    }
}