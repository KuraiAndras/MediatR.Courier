using System;
using System.Threading;

namespace MediatR.Courier
{
    public interface ICourier
    {
        void Subscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification;
        void UnSubscribe<TNotification>(Action<TNotification, CancellationToken> action) where TNotification : INotification;
    }
}
