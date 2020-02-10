using MediatR.Courier.TestResources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Courier.Tests
{
    public sealed class ConventionClientTests
    {
        private readonly MediatRCourier _courier;

        public ConventionClientTests() => _courier = new MediatRCourier();

        private sealed class ClientTypeTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { typeof(TestConventionClient1NoCancellation) };
                yield return new object[] { typeof(TestConventionClient1Cancellation) };
                yield return new object[] { typeof(TestInterfaceClient1Cancellation) };
                yield return new object[] { typeof(TestConventionClient3Mixed) };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(ClientTypeTestData))]
        public async Task ClientHandlersAreInvoked(Type clientType)
        {
            var client = (ICarryNotifications)Activator.CreateInstance(clientType, _courier);

            await InvokeAsync().ConfigureAwait(false);

            Assert.True(client.MessageReceivedCount == client.ProperlyImplementedHandleCount);
        }

        [Theory]
        [ClassData(typeof(ClientTypeTestData))]
        public async Task ClientHandlersAreNotInvokedAfterDisposeInvoked(Type clientType)
        {
            var client = (ICarryNotifications)Activator.CreateInstance(clientType, _courier);

            await InvokeAsync().ConfigureAwait(false);

            ((IDisposable)client).Dispose();

            await InvokeAsync().ConfigureAwait(false);

            Assert.True(client.MessageReceivedCount == client.ProperlyImplementedHandleCount);
        }

        [Fact]
        public async Task ConventionClientMethodInvoked()
        {
            var testClient = new TestConventionClient1NoCancellation(_courier);

            await InvokeAsync().ConfigureAwait(false);

            Assert.True(testClient.MessageReceivedCount == 1);
        }

        [Fact]
        public async Task ConventionClientAfterDisposeNotInvoked()
        {
            var testClient = new TestConventionClient1NoCancellation(_courier);

            await InvokeAsync().ConfigureAwait(false);

            testClient.Dispose();

            await InvokeAsync().ConfigureAwait(false);

            Assert.True(testClient.MessageReceivedCount == 1);
        }

        private async Task InvokeAsync() => await _courier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);
    }
}
