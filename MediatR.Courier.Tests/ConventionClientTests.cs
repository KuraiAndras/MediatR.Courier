using MediatR.Courier.TestResources;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Courier.Tests
{
    public sealed class ConventionClientTests
    {
        [Fact]
        public async Task ConventionClientMethodInvoked()
        {
            var courier = new MediatRCourier();

            var testClient = new TestConventionClient(courier);

            await courier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            Assert.True(testClient.MessageReceived);
        }

        [Fact]
        public async Task ConventionClientAfterDisposeNotInvoked()
        {
            var courier = new MediatRCourier();

            var testClient = new TestConventionClient(courier);

            testClient.Dispose();

            await courier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            Assert.False(testClient.MessageReceived);
        }
    }
}
