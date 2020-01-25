using MediatR.Courier.DependencyInjection;
using MediatR.Courier.Examples.Shared.Notifications;
using MediatR.Courier.Examples.Wpf.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MediatR.Courier.Examples.Wpf.Core.DependencyInjection
{
    public static class CompositionRoot
    {
        private static readonly IServiceProvider _serviceProvider = new ServiceCollection()
            .AddMediatR(AppDomain.CurrentDomain.GetAssemblies())
            .AddCourier(typeof(ExampleNotification).Assembly)
            .AddTransient<IExampleViewModel, ExampleViewModel>()
            .BuildServiceProvider();

        public static T Resolve<T>() => _serviceProvider.GetService<T>();
    }
}
