using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Courier;

/// <summary>
/// Provides extension methods for registering MediatR Courier services with the dependency injection container.
/// </summary>
public static class CourierInjector
{
    /// <summary>
    /// Registers MediatR Courier services with the dependency injection container using default configuration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="assemblies">The assemblies to scan for notification types.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCourier(this IServiceCollection services, params Assembly[] assemblies) =>
        services.AddCourier(_ => { }, assemblies);

    /// <summary>
    /// Registers MediatR Courier services with the dependency injection container using custom configuration.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configure">An action to configure the courier options.</param>
    /// <param name="assemblies">The assemblies to scan for notification types.</param>
    /// <returns>The service collection for chaining.</returns>
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
