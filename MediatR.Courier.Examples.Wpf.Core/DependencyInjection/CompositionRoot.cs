using MediatR.Courier.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MediatR.Courier.Examples.Wpf.Core.DependencyInjection
{
    public static class CompositionRoot
    {
        private static readonly IServiceProvider _serviceProvider = new ServiceCollection()
            .AddMediatR(AppDomain.CurrentDomain.GetAssemblies())
            .AddCourier(AppDomain.CurrentDomain.GetAssemblies())
            .BuildServiceProvider();

        public static T Resolve<T>() => _serviceProvider.GetService<T>();
    }
}
