using System;

namespace MediatR.Courier
{
    public interface ICourier
    {
        bool TrySubscribe<TNotification>(Action<TNotification> action) where TNotification : INotification;
        void UnSubscribe<TNotification>(Action<TNotification> action) where TNotification : INotification;
    }
}
