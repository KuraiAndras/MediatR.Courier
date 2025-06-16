using System.Collections;

using MediatR.Courier.Tests.TestResources;

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
            await Task.Delay(delayTime, cancellationToken);

            receivedMessage = true;
        }

        mediatRCourier.Subscribe<TestNotification>(NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);

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

        void NotificationAction(TestNotification _) => receivedMessage = true;

        mediatRCourier.Subscribe<TestNotification>(NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);

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

            await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken);
        }

        mediatRCourier.Subscribe<TestNotification>(NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);

        Assert.True(receivedMessage);
    }

    [Fact]
    public async Task UnSubscribedActionNotInvoked()
    {
        var mediatRCourier = new MediatRCourier(_options);

        var receivedMessageCount = 0;

        void NotificationAction(TestNotification _) => ++receivedMessageCount;

        mediatRCourier.Subscribe<TestNotification>(NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);

        mediatRCourier.UnSubscribe<TestNotification>(NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);

        Assert.Equal(1, receivedMessageCount);
    }

    [Fact]
    public async Task UnSubscribedAsyncActionNotInvoked()
    {
        var mediatRCourier = new MediatRCourier(_options);

        var receivedMessageCount = 0;

        async Task NotificationAction(TestNotification _, CancellationToken cancellationToken)
        {
            ++receivedMessageCount;

            await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken);
        }

        mediatRCourier.Subscribe<TestNotification>(NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);

        mediatRCourier.UnSubscribe<TestNotification>(NotificationAction);

        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);

        Assert.Equal(1, receivedMessageCount);
    }

    [Fact]
    public async Task Handlers_RunInParallel_WhenUseTaskWhenAllIsTrue()
    {
        var options = new CourierOptions { UseTaskWhenAll = true };
        var mediatRCourier = new MediatRCourier(options);

        var delays = new[] { 100, 200, 300 };
        var started = new bool[delays.Length];
        var completed = new bool[delays.Length];

        for (int i = 0; i < delays.Length; i++)
        {
            int idx = i;
            mediatRCourier.Subscribe<TestNotification>(async (_, ct) =>
            {
                started[idx] = true;
                await Task.Delay(delays[idx], ct);
                completed[idx] = true;
            });
        }

        var sw = System.Diagnostics.Stopwatch.StartNew();
        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);
        sw.Stop();

        Assert.All(started, Assert.True);
        Assert.All(completed, Assert.True);
        // Should complete in just over the max delay (parallel)
        Assert.InRange(sw.ElapsedMilliseconds, delays.Max(), delays.Max() + 150);
    }

    [Fact]
    public async Task Handlers_RunSequentially_WhenUseTaskWhenAllIsFalse()
    {
        var options = new CourierOptions { CaptureThreadContext = false, UseTaskWhenAll = false };
        var mediatRCourier = new MediatRCourier(options);

        var delays = new[] { 100, 200, 300 };
        var started = new bool[delays.Length];
        var completed = new bool[delays.Length];

        for (int i = 0; i < delays.Length; i++)
        {
            int idx = i;
            mediatRCourier.Subscribe<TestNotification>(async (_, ct) =>
            {
                started[idx] = true;
                await Task.Delay(delays[idx], ct);
                completed[idx] = true;
            });
        }

        var sw = System.Diagnostics.Stopwatch.StartNew();
        await mediatRCourier.Handle(new TestNotification(), CancellationToken.None);
        sw.Stop();

        Assert.All(started, s => Assert.True(s));
        Assert.All(completed, c => Assert.True(c));
        // Should complete in just over the sum of delays (sequential)
        Assert.InRange(sw.ElapsedMilliseconds, delays.Sum(), delays.Sum() + 150);
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
