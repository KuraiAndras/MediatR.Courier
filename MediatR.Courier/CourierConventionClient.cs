using System;
using System.Collections.Generic;

namespace MediatR.Courier
{
    public abstract class CourierConventionClient : IDisposable
    {
        private readonly ICourier _courier;
        private readonly IReadOnlyCollection<object> _actions;

        protected CourierConventionClient(ICourier courier)
        {
            _courier = courier;

            // TODO: Register actions on courier
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            foreach (var @delegate in _actions)
            {
                // TODO: unsubscribe
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
