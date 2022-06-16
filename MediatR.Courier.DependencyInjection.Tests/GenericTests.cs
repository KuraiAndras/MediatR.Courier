using FluentAssertions;
using MediatR.Courier.TestResources;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using static MediatR.Courier.DependencyInjection.Tests.TestHelper;

namespace MediatR.Courier.DependencyInjection.Tests;

public class GenericTests
{
    [Fact]
    public async Task MediatRHandlesGenericNotification()
    {
        var (sp, mediator, _) = SetUpCourier();

        await mediator.Publish(new GenericMessage<string>("Hello"));

        var messenger = sp.GetRequiredService<GenericMessenger>();

        messenger.CallCount.Should().Be(1);
    }
}
