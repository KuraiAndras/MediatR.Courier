using MediatR.Courier.TestResources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Courier.Tests
{
    public sealed class CourierTests
    {
        private sealed class AsyncTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { new TimeSpan(0L) };
                yield return new object[] { new TimeSpan(0, 0, 0, 0, 1) };
                yield return new object[] { new TimeSpan(0, 0, 0, 0, 10) };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(AsyncTestData))]
        public async Task AsyncActionMightCompleteInTime(TimeSpan delayTime)
        {
            var mediatRCourier = new MediatRCourier();

            var receivedMessage = false;

            async void NotificationAction(TestNotification _, CancellationToken __)
            {
                await Task.Delay(delayTime).ConfigureAwait(false);

                receivedMessage = true;
            }

            mediatRCourier.Subscribe<TestNotification>(NotificationAction);

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            if (delayTime.Ticks == 0)
            {
                Assert.True(receivedMessage);
            }
            else
            {
                Assert.False(receivedMessage);
            }
        }

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

            var receivedMessageCount = 0;

            void NotificationAction(TestNotification _, CancellationToken __) => ++receivedMessageCount;

            mediatRCourier.Subscribe<TestNotification>(NotificationAction);

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            mediatRCourier.UnSubscribe<TestNotification>(NotificationAction);

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            Assert.True(receivedMessageCount == 1);
        }
    }
}
