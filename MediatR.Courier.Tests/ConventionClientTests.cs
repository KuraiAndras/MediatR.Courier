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

#pragma warning disable SA1009 // Closing parenthesis should be spaced correctly
        [Theory]
        [ClassData(typeof(ClientTypeTestData))]
        public async Task ClientHandlersAreInvoked(Type clientType)
        {
            var client = (ICarryNotifications)Activator.CreateInstance(clientType, _courier)!;

            await InvokeNotificationsAsync().ConfigureAwait(false);

            Assert.True(client.MessageReceivedCount == client.ProperlyImplementedHandleCount);
        }

        [Theory]
        [ClassData(typeof(ClientTypeTestData))]
        public async Task ClientHandlersAreNotInvokedAfterDisposeInvoked(Type clientType)
        {
            var client = (ICarryNotifications)Activator.CreateInstance(clientType, _courier)!;

            await InvokeNotificationsAsync().ConfigureAwait(false);

            ((IDisposable)client).Dispose();

            await InvokeNotificationsAsync().ConfigureAwait(false);

            Assert.True(client.MessageReceivedCount == client.ProperlyImplementedHandleCount);
        }
#pragma warning restore SA1009 // Closing parenthesis should be spaced correctly

        [Fact]
        public async Task ConventionClientMethodInvoked()
        {
            var testClient = new TestConventionClient1NoCancellation(_courier);

            await InvokeNotificationsAsync().ConfigureAwait(false);

            Assert.True(testClient.MessageReceivedCount == testClient.ProperlyImplementedHandleCount);
        }

        [Fact]
        public async Task ConventionClientAfterDisposeNotInvoked()
        {
            var testClient = new TestConventionClient1NoCancellation(_courier);

            await InvokeNotificationsAsync().ConfigureAwait(false);

            testClient.Dispose();

            await InvokeNotificationsAsync().ConfigureAwait(false);

            Assert.True(testClient.MessageReceivedCount == testClient.ProperlyImplementedHandleCount);
        }

        private async Task InvokeNotificationsAsync()
        {
            await _courier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);
            await _courier.Handle(new TestNotification2(), CancellationToken.None).ConfigureAwait(false);
        }

        private sealed class ClientTypeTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { typeof(TestConventionClient1NoCancellation) };
                yield return new object[] { typeof(TestConventionClient1Cancellation) };
                yield return new object[] { typeof(TestInterfaceClient1Cancellation) };
                yield return new object[] { typeof(TestInterfaceClientWithTwoAsyncMethods) };
                yield return new object[] { typeof(TestConventionClientMixed) };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
