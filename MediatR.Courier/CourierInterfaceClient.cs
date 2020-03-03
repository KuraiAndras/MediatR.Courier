using MediatR.Courier.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediatR.Courier
{
    public abstract class CourierInterfaceClient : IDisposable
    {
        private readonly ICourier _courier;
        private readonly ICollection<Delegate> _handlers;

        protected CourierInterfaceClient(ICourier courier)
        {
            _courier = courier;
            var subType = GetType();

            _handlers = subType.GetInterfaces()
                .Where(i =>
                    i.IsGenericType
                    && (i.GetGenericTypeDefinition() != typeof(ICourierNotificationHandler<>)
                        || i.GetGenericTypeDefinition() != typeof(ICourierNotificationHandlerAsync<>)))
                .SelectMany(i => i
                    .GetMethods()
                    .Select(methodInfo =>
                    {
                        var handler = Delegate.CreateDelegate(methodInfo.CreateCourierHandlerType(), this, methodInfo);

                        _courier.InvokeCourierMethod(nameof(ICourier.Subscribe), handler.Method, handler);

                        return handler;
                    }))
                .ToList();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            foreach (var handler in _handlers)
            {
                _courier.InvokeCourierMethod(nameof(ICourier.UnSubscribe), handler.Method, handler);
            }

            _handlers.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
