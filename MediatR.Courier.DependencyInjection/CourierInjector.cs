using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace MediatR.Courier.DependencyInjection
{
    public static class CourierInjector
    {
        public static IServiceCollection AddCourier(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddSingleton(typeof(MediatRCourier));
            services.AddSingleton(typeof(ICourier), serviceProvider => serviceProvider.GetService(typeof(MediatRCourier)));

            var notificationType = typeof(INotification);
            var notificationHandlerType = typeof(INotificationHandler<>);

            foreach (var notificationImplementation in assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Any(i => i == notificationType)))
            {
                services.AddSingleton(
                    notificationHandlerType.MakeGenericType(notificationImplementation),
                    serviceProvider => serviceProvider.GetService(typeof(MediatRCourier)));
            }

            return services;
        }
    }
}
