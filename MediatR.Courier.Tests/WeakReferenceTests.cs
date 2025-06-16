using MediatR.Courier.Tests.TestResources;

namespace MediatR.Courier.Tests;

public sealed class WeakReferenceTests
{
    [Fact]
    public async Task SubscribedAsyncActionInvoked()
    {
        var mediatRCourier = new MediatRCourier(new());

        var handler = new Handler(new());

        mediatRCourier.SubscribeWeak<TestNotification>(handler.NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);

        Assert.Equal(1, handler.ReceivedMessageCount);
    }

    [Fact]
    public async Task UnSubscribedAsyncActionNotInvoked()
    {
        var mediatRCourier = new MediatRCourier(new());

        var handler = new Handler(new());

        mediatRCourier.SubscribeWeak<TestNotification>(handler.NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);

        mediatRCourier.UnSubscribe<TestNotification>(handler.NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);

        Assert.Equal(1, handler.ReceivedMessageCount);
    }

    [Fact]
    public async Task UnSubscribedAsyncActionNotInvokedButOthersAre()
    {
        var mediatRCourier = new MediatRCourier(new());

        var handler = new Handler(new(), new());
        var handler2 = new Handler(new(), new());

        mediatRCourier.SubscribeWeak<TestNotification>(handler.NotificationAction);
        mediatRCourier.SubscribeWeak<TestNotification>(handler.NotificationAction2);
        mediatRCourier.SubscribeWeak<TestNotification>(handler2.NotificationAction);
        mediatRCourier.SubscribeWeak<TestNotification>(handler2.NotificationAction2);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

        mediatRCourier.UnSubscribe<TestNotification>(handler.NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

        Assert.Equal(1, handler.ReceivedMessageCount);
        Assert.Equal(2, handler.ReceivedMessageCount2);
        Assert.Equal(2, handler2.ReceivedMessageCount);
        Assert.Equal(2, handler2.ReceivedMessageCount2);
    }

    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S1215:\"GC.Collect\" should not be called", Justification = "We want to test behavior around GC collection")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "False positive")]
    public async Task CollectedReferenceIsNotInvoked()
    {
        var mediatRCourier = new MediatRCourier(new());

        var counter = new Counter();
        var handler = new Handler(counter);

        mediatRCourier.SubscribeWeak<TestNotification>(handler.NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);

#pragma warning disable S1854 // Unused assignments should be removed
        handler = null;
#pragma warning restore S1854 // Unused assignments should be removed

        // We wait for some time to make sure that the gc actually collects and runs
        await Task.Delay(TimeSpan.FromSeconds(1));
        GC.Collect(2, GCCollectionMode.Forced, true);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);

        Assert.Equal(1, counter.ReceivedMessageCount);
    }

    private sealed class Counter
    {
        public int ReceivedMessageCount { get; set; }
    }

    private sealed class Handler
    {
        private readonly Counter _counter;
        private readonly Counter? _counter2;

        public Handler(Counter counter, Counter? counter2 = null)
        {
            _counter = counter;
            _counter2 = counter2;
        }

#pragma warning disable S1172 // Unused method parameters should be removed
        public async Task NotificationAction(TestNotification _, CancellationToken cancellationToken)
#pragma warning restore S1172 // Unused method parameters should be removed
        {
            _counter.ReceivedMessageCount++;

            await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken);
        }

#pragma warning disable S1172 // Unused method parameters should be removed
        public async Task NotificationAction2(TestNotification _, CancellationToken cancellationToken)
#pragma warning restore S1172 // Unused method parameters should be removed
        {
            if (_counter2 is null) return;

            _counter2.ReceivedMessageCount++;

            await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);
        }

        public int ReceivedMessageCount => _counter.ReceivedMessageCount;
        public int? ReceivedMessageCount2 => _counter2?.ReceivedMessageCount;
    }
}
