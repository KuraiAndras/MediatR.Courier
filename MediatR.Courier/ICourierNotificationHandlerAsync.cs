using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier
{
    public interface ICourierNotificationHandlerAsync<in TNotification> where TNotification : INotification
    {
        Task HandleAsync(TNotification notification, CancellationToken cancellationToken = default);
    }
}