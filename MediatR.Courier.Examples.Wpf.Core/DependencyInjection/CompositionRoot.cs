using MediatR.Courier.Examples.Shared;
using MediatR.Courier.Examples.Wpf.Core.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Courier.Examples.Wpf.Core.DependencyInjection;

public static class CompositionRoot
{
    private static readonly IServiceProvider ServiceProvider = new ServiceCollection()
        .AddMediatR(c => c.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()))
        .AddCourier(typeof(SharedMarkerType).Assembly)
        .AddTransient<IExampleViewModel, ExampleViewModel>()
        .BuildServiceProvider();

    public static T? Resolve<T>() => ServiceProvider.GetService<T>();
}