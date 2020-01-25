using MediatR.Courier.DependencyInjection;
using MediatR.Courier.Examples.Wpf.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using MediatR.Courier.Examples.Shared.Notifications;

namespace MediatR.Courier.Examples.Wpf.Core.DependencyInjection
{
    public static class CompositionRoot
    {
        private static readonly IServiceProvider _serviceProvider;

        static CompositionRoot()
        {
            var services = new ServiceCollection()
                .AddMediatR(AppDomain.CurrentDomain.GetAssemblies())
                .AddCourier(typeof(ExampleNotification).Assembly)
                .AddTransient<IExampleViewModel, ExampleViewModel>();
            _serviceProvider = services
                .BuildServiceProvider();
        }

        public static T Resolve<T>() => _serviceProvider.GetService<T>();
    }
}
