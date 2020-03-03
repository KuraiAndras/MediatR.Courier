using MediatR.Courier.Exceptions;
using System;
using System.Linq;
using System.Reflection;

namespace MediatR.Courier.Extensions
{
    internal static class CourierExtensions
    {
        internal static MethodInfo GetCourierMethod(this ICourier courier, string methodName, bool actionHasCancellation, Type notificationType)
        {
            var parameterGenericArgumentCount = actionHasCancellation ? 2 : 1;

            var baseMethod = courier
                .GetType()
                .GetMethods()
                .SingleOrDefault(m =>
                {
                    if (m.Name != methodName) return false;

                    var parameters = m.GetParameters();

                    if (parameters.Length != 1) return false;

                    var parameter = parameters[0];

                    if (!parameter.ParameterType.IsGenericType) return false;

                    return parameter.ParameterType.GetGenericArguments().Length == parameterGenericArgumentCount;
                });

            if (baseMethod is null) throw new MethodNotImplementedException($"{nameof(ICourier)} does not have a method named {nameof(ICourier.Subscribe)}");

            return baseMethod.MakeGenericMethod(notificationType);
        }
    }
}
