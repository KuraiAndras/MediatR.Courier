using MediatR.Courier.TestResources;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Courier.Tests
{
    public class CourierTests
    {
        [Fact]
        public async Task SubscribedActionInvoked()
        {
            var mediatRCourier = new MediatRCourier();

            var receivedMessage = false;

            void NotificationAction(TestNotification _, CancellationToken __) => receivedMessage = true;

            mediatRCourier.Subscribe<TestNotification>(NotificationAction);

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            Assert.True(receivedMessage);
        }

        [Fact]
        public async Task UnSubscribedActionNotInvoked()
        {
            var mediatRCourier = new MediatRCourier();

            var receivedMessage = false;

            void NotificationAction(TestNotification _, CancellationToken __) => receivedMessage = true;

            mediatRCourier.Subscribe<TestNotification>(NotificationAction);
            mediatRCourier.UnSubscribe<TestNotification>(NotificationAction);

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            Assert.False(receivedMessage);
        }
    }
}
