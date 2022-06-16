using FluentAssertions;
using Xunit;

namespace MediatR.Courier.Tests;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests")]
public class GenericTests
{
    public record Message<T>(T Data) : INotification;

    [Fact]
    public async Task Generic_Message_Is_Handled()
    {
        var courier = new MediatRCourier(new() { CaptureThreadContext = false });

        string? data = null;

        void Handler(Message<string> message) => data = message.Data;

        courier.Subscribe<Message<string>>(Handler);

        const string testData = "Hello";

        await courier.Handle(new Message<string>(testData), CancellationToken.None);

        courier.UnSubscribe<Message<string>>(Handler);

        data.Should().Be(testData);
    }
}
