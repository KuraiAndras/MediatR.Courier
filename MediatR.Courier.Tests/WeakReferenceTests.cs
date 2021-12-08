using MediatR.Courier.TestResources;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Courier.Tests
{
    public sealed class WeakReferenceTests
    {
        [Fact]
        public async Task SubscribedAsyncActionInvoked()
        {
            var mediatRCourier = new MediatRCourier(new());

            var handler = new Handler(new());

            mediatRCourier.SubscribeWeak<TestNotification>(handler.NotificationAction);

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            Assert.Equal(1, handler.ReceivedMessageCount);
        }

        [Fact]
        public async Task UnSubscribedAsyncActionNotInvoked()
        {
            var mediatRCourier = new MediatRCourier(new());

            var handler = new Handler(new());

            mediatRCourier.SubscribeWeak<TestNotification>(handler.NotificationAction);

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            mediatRCourier.UnSubscribe<TestNotification>(handler.NotificationAction);

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            Assert.Equal(1, handler.ReceivedMessageCount);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "We want to test behavior around GC collection")]
        public async Task CollectedReferenceIsNotInvoked()
        {
            var mediatRCourier = new MediatRCourier(new());

            var counter = new Counter();
            var handler = new Handler(counter);

            mediatRCourier.SubscribeWeak<TestNotification>(handler.NotificationAction);

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            handler = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

            Assert.Equal(1, counter.ReceivedMessageCount);
        }

        private sealed class Counter
        {
            public int ReceivedMessageCount { get; set; }
        }

        private sealed class Handler
        {
            private readonly Counter _counter;

            public Handler(Counter counter) => _counter = counter;

#pragma warning disable S1172 // Unused method parameters should be removed
            public async Task NotificationAction(TestNotification _, CancellationToken cancellationToken)
#pragma warning restore S1172 // Unused method parameters should be removed
            {
                _counter.ReceivedMessageCount++;

                await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);
            }

            public int ReceivedMessageCount => _counter.ReceivedMessageCount;
        }
    }
}
