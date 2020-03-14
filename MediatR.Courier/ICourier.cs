using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier
{
    public interface ICourier
    {
        void Subscribe<TNotification>(Action<TNotification> handler)
            where TNotification : INotification;

        void Subscribe<TNotification>(Action<TNotification, CancellationToken> handler)
            where TNotification : INotification;

        void Subscribe<TNotification>(Func<TNotification, Task> handler)
            where TNotification : INotification;

        void Subscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
            where TNotification : INotification;

        void UnSubscribe<TNotification>(Action<TNotification> handler)
            where TNotification : INotification;

        void UnSubscribe<TNotification>(Action<TNotification, CancellationToken> handler)
            where TNotification : INotification;

        void UnSubscribe<TNotification>(Func<TNotification, Task> handler)
            where TNotification : INotification;

        void UnSubscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
            where TNotification : INotification;
    }
}
