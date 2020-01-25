using MediatR.Courier.Examples.Wpf.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace MediatR.Courier.Examples.Wpf.Core.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            foreach (var viewModelType in AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(ViewModelBase))))
            {
                foreach (var @interface in viewModelType.GetInterfaces())
                {
                    services.AddTransient(@interface, viewModelType);
                }
            }

            return services;
        }
    }
}
