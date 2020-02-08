using System;
using System.Linq;
using System.Reflection;

namespace MediatR.Courier.Extensions
{
    internal static class ICourierExtensions
    {
        internal static MethodInfo GetCourierMethod(this ICourier courier, CourierMethodName methodName, CourierMethodType methodType)
        {
            string methodNameString;
            switch (methodName)
            {
                case CourierMethodName.Subscribe:
                    methodNameString = nameof(ICourier.Subscribe);
                    break;
                case CourierMethodName.UnSubscribe:
                    methodNameString = nameof(ICourier.UnSubscribe);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(methodName), methodName, null);
            }

            int genericArgumentCount;
            switch (methodType)
            {
                case CourierMethodType.Cancellation:
                    genericArgumentCount = 2;
                    break;
                case CourierMethodType.NoCancellation:
                    genericArgumentCount = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(methodType), methodType, null);
            }

            return courier
                .GetType()
                .GetMethods()
                .SingleOrDefault(m =>
                {
                    if (m.Name != methodNameString) return false;

                    var parameters = m.GetParameters();

                    if (parameters.Length != 1) return false;

                    var parameter = parameters[0];

                    if (!parameter.ParameterType.IsGenericType) return false;

                    return parameter.ParameterType.GetGenericArguments().Length == genericArgumentCount;
                });
        }
    }
}
