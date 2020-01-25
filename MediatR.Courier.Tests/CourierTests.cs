using MediatR.Courier.DependencyInjection;
using MediatR.Courier.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace MediatR.Courier.Tests
{
    public class CourierTests
    {
        [Fact]
        public async Task TestBasicUsage()
        {
            var services = new ServiceCollection()
                .AddMediatR(Assembly.GetExecutingAssembly())
                .AddCourier(Assembly.GetExecutingAssembly());

            var serviceProvider = services.BuildServiceProvider();

            var mediator = serviceProvider.GetService<IMediator>();
            var courier = serviceProvider.GetService<ICourier>();

            var receivedMessage = string.Empty;

            void NotificationAction(ExampleNotification n) => receivedMessage = n.Message;

            courier.TrySubscribe<ExampleNotification>(NotificationAction);

            await mediator.Send(new ExampleRequest { Message = "Test1: " });

            Assert.True(!string.IsNullOrEmpty(receivedMessage));

            courier.UnSubscribe<ExampleNotification>(NotificationAction);
        }
    }
}
