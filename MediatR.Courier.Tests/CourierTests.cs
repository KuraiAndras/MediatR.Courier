using MediatR.Courier.TestResources;
using System.Collections;
using Xunit;

namespace MediatR.Courier.Tests;

public sealed class CourierTests
{
    private readonly CourierOptions _options = new() { CaptureThreadContext = false };

    [Theory]
    [ClassData(typeof(AsyncTestData))]
    public async Task AsyncVoidActionMightCompleteInTime(TimeSpan delayTime)
    {
        var mediatRCourier = new MediatRCourier(_options);

        var receivedMessage = false;

        async void NotificationAction(TestNotification _, CancellationToken cancellationToken)
        {
            await Task.Delay(delayTime, cancellationToken).ConfigureAwait(false);

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
        var mediatRCourier = new MediatRCourier(_options);

        var receivedMessage = false;

        void NotificationAction(TestNotification _, CancellationToken __) => receivedMessage = true;

        mediatRCourier.Subscribe<TestNotification>(NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

        Assert.True(receivedMessage);
    }

    [Fact]
    public async Task SubscribedAsyncActionInvoked()
    {
        var mediatRCourier = new MediatRCourier(_options);

        var receivedMessage = false;

        async Task NotificationAction(TestNotification _, CancellationToken cancellationToken)
        {
            receivedMessage = true;

            await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);
        }

        mediatRCourier.Subscribe<TestNotification>(NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

        Assert.True(receivedMessage);
    }

    [Fact]
    public async Task UnSubscribedActionNotInvoked()
    {
        var mediatRCourier = new MediatRCourier(_options);

        var receivedMessageCount = 0;

        void NotificationAction(TestNotification _, CancellationToken __) => ++receivedMessageCount;

        mediatRCourier.Subscribe<TestNotification>(NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

        mediatRCourier.UnSubscribe<TestNotification>(NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

        Assert.True(receivedMessageCount == 1);
    }

    [Fact]
    public async Task UnSubscribedAsyncActionNotInvoked()
    {
        var mediatRCourier = new MediatRCourier(_options);

        var receivedMessageCount = 0;

        async Task NotificationAction(TestNotification _, CancellationToken cancellationToken)
        {
            ++receivedMessageCount;

            await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken).ConfigureAwait(false);
        }

        mediatRCourier.Subscribe<TestNotification>(NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

        mediatRCourier.UnSubscribe<TestNotification>(NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None).ConfigureAwait(false);

        Assert.True(receivedMessageCount == 1);
    }

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
}
