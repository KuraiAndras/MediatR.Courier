using MediatR.Courier.DependencyInjection;
using MediatR.Courier.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Courier.Tests
{
    public class CourierTests
    {
        private static (IServiceProvider serviceProvider, IMediator mediator, ICourier courier) SetUpCourier()
        {
            var services = new ServiceCollection()
                .AddMediatR(Assembly.GetExecutingAssembly())
                .AddCourier(Assembly.GetExecutingAssembly());

            var serviceProvider = services.BuildServiceProvider();

            var mediator = serviceProvider.GetService<IMediator>();
            var courier = serviceProvider.GetService<ICourier>();

            return (serviceProvider, mediator, courier);
        }

        [Fact]
        public void TestServiceRegistration()
        {
            var (serviceProvider, _, _) = SetUpCourier();

            var handlerInstance = serviceProvider.GetServices<INotificationHandler<ExampleNotification>>();

            Assert.NotNull(handlerInstance);
        }

        [Fact]
        public void TestHandlerCourierSameInstances()
        {
            var (serviceProvider, _, courier) = SetUpCourier();

            var mediatRCourier = serviceProvider.GetService<MediatRCourier>();
            var handlerInstance = serviceProvider.GetService<INotificationHandler<ExampleNotification>>();

            Assert.Same(mediatRCourier, handlerInstance);
            Assert.Same(mediatRCourier, courier);
        }

        [Fact]
        public async Task TestSubscribe()
        {
            var (_, mediator, courier) = SetUpCourier();

            var receivedMessage = string.Empty;
            const string baseMessage = "Test: ";

            void NotificationAction(ExampleNotification n) => receivedMessage = n.Message;

            courier.TrySubscribe<ExampleNotification>(NotificationAction);

            await mediator.Send(new ExampleRequest { Message = baseMessage });

            Assert.True(!string.IsNullOrEmpty(receivedMessage) && receivedMessage.Length > baseMessage.Length);
        }

        [Fact]
        public async Task TestUnSubscribe()
        {
            var (_, mediator, courier) = SetUpCourier();

            var receivedMessage = string.Empty;

            void NotificationAction(ExampleNotification n) => receivedMessage = n.Message;

            courier.TrySubscribe<ExampleNotification>(NotificationAction);
            courier.UnSubscribe<ExampleNotification>(NotificationAction);

            await mediator.Send(new ExampleRequest { Message = "Test: " });

            Assert.True(string.IsNullOrEmpty(receivedMessage));
        }
    }
}
