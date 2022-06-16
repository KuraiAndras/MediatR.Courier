using MediatR.Courier.Extensions;

namespace MediatR.Courier;

public abstract class CourierConventionClient : IDisposable
{
    private readonly ICourier _courier;
    private readonly ICollection<Delegate> _handlers;

    protected CourierConventionClient(ICourier courier)
    {
        _courier = courier;

        var subType = GetType();

        var baseNotificationType = typeof(INotification);

        _handlers = subType.GetMethods()
            .Where(m =>
            {
                if (m.ReturnType != typeof(void) && m.ReturnType != typeof(Task)) return false;

                var parameters = m.GetParameters();

                if (parameters.Length > 2 || parameters.Length == 0) return false;

                return baseNotificationType.IsAssignableFrom(parameters[0].ParameterType);
            })
            .Select(method =>
            {
                var handler = Delegate.CreateDelegate(method.CreateCourierHandlerType(), this, method);

                _courier.InvokeCourierMethod(nameof(ICourier.Subscribe), method, handler);

                return handler;
            })
            .ToList();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
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
}
