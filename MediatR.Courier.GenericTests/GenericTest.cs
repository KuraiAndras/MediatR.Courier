using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Courier.GenericTests;

public class GenericTest
{
    public record GenericMessage<T>(T Data) : INotification;


    public class GenericHandler<T> : INotificationHandler<GenericMessage<T>>
    {
        public Task Handle(GenericMessage<T> notification, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    [Fact]
    public void This_Should_Not_Throw()
    {
        var services = new ServiceCollection()
            .AddMediatR(typeof(GenericTest).Assembly);

        var serviceProvider = services.BuildServiceProvider();

        var mediator = serviceProvider.GetRequiredService<IMediator>();

        mediator.Publish(new GenericMessage<string>("Hello"));
    }

    [Fact]
    public void This_Should_Not_Throw_Also()
    {
        var services = new ServiceCollection()
            .AddMediatR(typeof(GenericTest).Assembly)
            .AddTransient(typeof(INotificationHandler<>), typeof(GenericHandler<>));

        var serviceProvider = services.BuildServiceProvider();

        var mediator = serviceProvider.GetRequiredService<IMediator>();

        mediator.Publish(new GenericMessage<string>("Hello"));
    }
}
