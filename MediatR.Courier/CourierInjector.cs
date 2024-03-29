﻿using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Courier;

public static class CourierInjector
{
    public static IServiceCollection AddCourier(this IServiceCollection services, params Assembly[] assemblies) =>
        services.AddCourier(_ => { }, assemblies);

    public static IServiceCollection AddCourier(this IServiceCollection services, Action<CourierOptions> configure, params Assembly[] assemblies)
    {
        var options = new CourierOptions();
        configure(options);

        services.AddSingleton(options);

        services.AddSingleton(typeof(MediatRCourier));
        services.AddSingleton(typeof(ICourier), serviceProvider => serviceProvider.GetRequiredService(typeof(MediatRCourier)));

        var notificationType = typeof(INotification);
        var notificationHandlerType = typeof(INotificationHandler<>);

        foreach (var notificationImplementation in assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetInterfaces().Any(i => i == notificationType)))
        {
            services.AddSingleton(
                notificationHandlerType.MakeGenericType(notificationImplementation),
                serviceProvider => serviceProvider.GetRequiredService(typeof(MediatRCourier)));
        }

        return services;
    }
}