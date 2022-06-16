using FluentAssertions;
using MediatR.Courier.TestResources;
using Xunit;

namespace MediatR.Courier.Tests;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests")]
public class GenericTests
{
    [Fact]
    public async Task Generic_Message_Is_Handled()
    {
        var courier = new MediatRCourier(new() { CaptureThreadContext = false });

        string? data = null;

        void Handler(GenericMessage<string> message) => data = message.Data;

        courier.Subscribe<GenericMessage<string>>(Handler);

        const string testData = "Hello";

        await courier.Handle(new GenericMessage<string>(testData), CancellationToken.None);

        courier.UnSubscribe<GenericMessage<string>>(Handler);

        data.Should().Be(testData);
    }
}
