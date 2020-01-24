using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Courier.DependencyInjection
{
    public static class CourierInjector
    {
        public static IServiceCollection AddCourier(this IServiceCollection services)
        {
            services.AddSingleton(typeof(INotificationHandler<>), typeof(MediatRCourier<>));

            return services;
        }
    }
}
