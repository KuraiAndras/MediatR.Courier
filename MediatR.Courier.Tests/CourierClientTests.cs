using MediatR.Courier.TestResources;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Courier.Tests
{
    public sealed class CourierClientTests
    {
        [Fact]
        public async Task SubscribedMethodInvoked()
        {
            var mediatRCourier = new MediatRCourier();

            var testClient = new TestInterfaceClientWithOneMethod(mediatRCourier);

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            Assert.True(testClient.MessageReceived);
        }

        [Fact]
        public async Task MultipleSubscribedMethodsInvoked()
        {
            var mediatRCourier = new MediatRCourier();

            var testClient = new TestInterfaceClientWithTwoMethods(mediatRCourier);

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);
            await mediatRCourier.Handle(new TestNotification2(), CancellationToken.None).ConfigureAwait(false);

            Assert.True(testClient.MessageReceived);
            Assert.True(testClient.MessageReceived2);
        }

        [Fact]
        public async Task UnSubscribedMethodNotInvoked()
        {
            var mediatRCourier = new MediatRCourier();

            var testClient = new TestInterfaceClientWithOneMethod(mediatRCourier);

            testClient.Dispose();

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            Assert.False(testClient.MessageReceived);
        }

        [Fact]
        public async Task MultipleUnSubscribedMethodsNotInvoked()
        {
            var mediatRCourier = new MediatRCourier();

            var testClient = new TestInterfaceClientWithTwoMethods(mediatRCourier);

            testClient.Dispose();

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);
            await mediatRCourier.Handle(new TestNotification2(), CancellationToken.None).ConfigureAwait(false);

            Assert.False(testClient.MessageReceived);
            Assert.False(testClient.MessageReceived2);
        }
    }
}
