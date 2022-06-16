using MediatR.Courier.TestResources;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Courier.DependencyInjection.Tests;

public static class TestHelper
{
    public static (IServiceProvider serviceProvider, IMediator mediator, ICourier courier) SetUpCourier(params Action<IServiceCollection>[] otherRegistrations)
    {
        var services = new ServiceCollection()
            .AddMediatR(typeof(TestResourcesMarkerType))
            .AddCourier(typeof(TestResourcesMarkerType).Assembly)
            .AddSingleton<GenericMessenger>();

        foreach (var registration in otherRegistrations)
        {
            registration(services);
        }

        var serviceProvider = services.BuildServiceProvider();

        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var courier = serviceProvider.GetRequiredService<ICourier>();

        return (serviceProvider, mediator, courier);
    }
}
