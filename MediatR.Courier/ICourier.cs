using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier
{
    public interface ICourier
    {
        void Subscribe<TNotification>(Action<TNotification> action) where TNotification : INotification;
        void Subscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification;
        void Subscribe<TNotification>(Func<TNotification, Task> action) where TNotification : INotification;
        void Subscribe<TNotification>(Func<TNotification, CancellationToken, Task> action) where TNotification : INotification;
        void UnSubscribe<TNotification>(Action<TNotification> action) where TNotification : INotification;
        void UnSubscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification;
        void UnSubscribe<TNotification>(Func<TNotification, Task> action) where TNotification : INotification;
        void UnSubscribe<TNotification>(Func<TNotification, CancellationToken, Task> action) where TNotification : INotification;
    }
}
