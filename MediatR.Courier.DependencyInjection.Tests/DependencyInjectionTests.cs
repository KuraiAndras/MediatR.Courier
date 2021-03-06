using MediatR.Courier.TestResources;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Courier.DependencyInjection.Tests
{
    public class DependencyInjectionTests
    {
        [Fact]
        public void NotificationHandlerIsRegistered()
        {
            var (serviceProvider, _, _) = SetUpCourier();

            var handlerInstance = serviceProvider.GetServices<INotificationHandler<TestNotification>>();

            Assert.NotNull(handlerInstance);
        }

        [Fact]
        public void CourierAndHandlerHasSameInstance()
        {
            var (serviceProvider, _, _) = SetUpCourier();

            var mediatRCourier = serviceProvider.GetService<MediatRCourier>();
            var handlerInstance = serviceProvider.GetService<INotificationHandler<TestNotification>>();
            var courier = serviceProvider.GetService<ICourier>();

            Assert.Same(mediatRCourier, handlerInstance);
            Assert.Same(mediatRCourier, courier);
        }

        [Fact]
        public async Task MediatRFindsHandler()
        {
            var (_, mediator, courier) = SetUpCourier();

            var receivedMessage = false;

            void NotificationAction(TestNotification _, CancellationToken __) => receivedMessage = true;

            courier.Subscribe<TestNotification>(NotificationAction);

            await mediator.Publish(new TestNotification()).ConfigureAwait(false);

            Assert.True(receivedMessage);
        }

        private static (IServiceProvider serviceProvider, IMediator mediator, ICourier courier) SetUpCourier()
        {
            var services = new ServiceCollection()
                .AddMediatR(typeof(TestResourcesMarkerType))
                .AddCourier(typeof(TestResourcesMarkerType).Assembly);

            var serviceProvider = services.BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var courier = serviceProvider.GetRequiredService<ICourier>();

            return (serviceProvider, mediator, courier);
        }
    }
}
