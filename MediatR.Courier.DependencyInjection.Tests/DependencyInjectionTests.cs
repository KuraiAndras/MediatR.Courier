using MediatR.Courier.TestResources;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Courier.DependencyInjection.Tests
{
    public class DependencyInjectionTests
    {
        private static (IServiceProvider serviceProvider, IMediator mediator, ICourier courier) SetUpCourier()
        {
            var services = new ServiceCollection()
                .AddMediatR(typeof(TestResourcesMarkerType))
                .AddCourier(typeof(TestResourcesMarkerType).Assembly);

            var serviceProvider = services.BuildServiceProvider();

            var mediator = serviceProvider.GetService<IMediator>();
            var courier = serviceProvider.GetService<ICourier>();

            return (serviceProvider, mediator, courier);
        }

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

            void NotificationAction(TestNotification _) => receivedMessage = true;

            courier.TrySubscribe<TestNotification>(NotificationAction);

            await mediator.Publish(new TestNotification()).ConfigureAwait(false);

            Assert.True(receivedMessage);
        }
    }
}
