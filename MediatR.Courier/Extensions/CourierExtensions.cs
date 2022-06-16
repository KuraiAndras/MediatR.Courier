using MediatR.Courier.Exceptions;
using System.Reflection;

namespace MediatR.Courier.Extensions;

internal static class CourierExtensions
{
    internal static void InvokeCourierMethod(
        this ICourier courier,
        string courierMethodName,
        MethodInfo handler,
        Delegate handlerDelegate)
    {
        var methodGenericArguments = new List<Type>(handler.GetParameters().Select(p => p.ParameterType));
        if (handler.ReturnType == typeof(Task)) methodGenericArguments.Add(handler.ReturnType);

        var baseMethod = typeof(ICourier)
            .GetMethods()
            .SingleOrDefault(m =>
            {
                if (m.Name != courierMethodName) return false;

                var parameters = m.GetParameters();

                if (parameters.Length != 1) return false;

                var parameter = parameters[0];

                if (!parameter.ParameterType.IsGenericType) return false;

                var parameterGenericArguments = parameter
                    .ParameterType
                    .GetGenericArguments();

                return parameterGenericArguments
                    .Skip(1)
                    .SequenceEquivalent(methodGenericArguments
                        .Skip(1));
            });

        if (baseMethod is null) throw new MethodNotImplementedException($"{nameof(ICourier)} does not have a method named {nameof(courierMethodName)}");

        var subscribeMethod = baseMethod.MakeGenericMethod(methodGenericArguments[0]);

        subscribeMethod.Invoke(courier, new object[] { handlerDelegate });
    }

    internal static Type CreateCourierHandlerType(this MethodInfo methodInfo)
    {
        var parameters = new List<Type>(methodInfo.GetParameters().Select(p => p.ParameterType));
        if (methodInfo.ReturnType == typeof(Task)) parameters.Add(methodInfo.ReturnType);
        var notificationType = parameters[0];

        return parameters.Count switch
        {
            1 => typeof(Action<>).MakeGenericType(notificationType),

            2 when parameters[1] == typeof(Task) => typeof(Func<,>).MakeGenericType(notificationType, typeof(Task)),

            2 => typeof(Action<,>).MakeGenericType(notificationType, typeof(CancellationToken)),

            3 => typeof(Func<,,>).MakeGenericType(notificationType, typeof(CancellationToken), typeof(Task)),

            _ => throw new UnknownMethodException(methodInfo.Name),
        };
    }

    private static bool SequenceEquivalent<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T>? comparer = null)
    {
        var cnt = comparer is null
            ? new Dictionary<T, int>()
            : new Dictionary<T, int>(comparer);

        foreach (var s in first)
        {
            if (cnt.ContainsKey(s))
            {
                cnt[s]++;
            }
            else
            {
                cnt.Add(s, 1);
            }
        }

        foreach (var s in second)
        {
            if (cnt.ContainsKey(s))
            {
                cnt[s]--;
            }
            else
            {
                return false;
            }
        }

        return cnt.Values.All(c => c == 0);
    }
}
