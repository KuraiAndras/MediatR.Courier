using MediatR.Courier.TestResources;

using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Courier.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void NotificationHandlerIsRegistered()
    {
        var (serviceProvider, _, _) = SetUpCourier();

        var handlerInstance = serviceProvider.GetServices<INotificationHandler<TestNotification>>();

        Assert.NotNull(handlerInstance);
    }

    [Fact]
    public void CourierAndHandlerHasSameInstance()
    {
        var (serviceProvider, _, _) = SetUpCourier();

        var mediatRCourier = serviceProvider.GetService<MediatRCourier>();
        var handlerInstance = serviceProvider.GetService<INotificationHandler<TestNotification>>();
        var courier = serviceProvider.GetService<ICourier>();

        Assert.Same(mediatRCourier, handlerInstance);
        Assert.Same(mediatRCourier, courier);
    }

    [Fact]
    public async Task MediatRFindsHandler()
    {
        var (_, mediator, courier) = SetUpCourier();

        var receivedMessage = false;

        void NotificationAction(TestNotification _) => receivedMessage = true;

        courier.Subscribe<TestNotification>(NotificationAction);

        await mediator.Publish(new TestNotification()).ConfigureAwait(false);

        Assert.True(receivedMessage);
    }

    private static (IServiceProvider serviceProvider, IMediator mediator, ICourier courier) SetUpCourier()
    {
        var services = new ServiceCollection()
            .AddMediatR(c => c.RegisterServicesFromAssemblyContaining(typeof(TestResourcesMarkerType)))
            .AddCourier(typeof(TestResourcesMarkerType).Assembly);

        var serviceProvider = services.BuildServiceProvider();

        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var courier = serviceProvider.GetRequiredService<ICourier>();

        return (serviceProvider, mediator, courier);
    }
}