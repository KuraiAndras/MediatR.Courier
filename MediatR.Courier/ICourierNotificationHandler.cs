using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier
{
    public interface ICourierNotificationHandler<in TNotification> where TNotification : INotification
    {
        Task Handle(TNotification notification, CancellationToken cancellationToken = default);
    }
}