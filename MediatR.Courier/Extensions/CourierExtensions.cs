using MediatR.Courier.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Courier.Extensions
{
    internal static class CourierExtensions
    {
        internal static void InvokeCourierMethod(
            this ICourier courier,
            string courierMethodName,
            MethodInfo handler,
            Delegate handlerDelegate)
        {
            var methodGenericArguments = handler.GetParameters();

            var baseMethod = typeof(ICourier)
                .GetMethods()
                .SingleOrDefault(m =>
                {
                    if (m.Name != courierMethodName) return false;

                    var parameters = m.GetParameters();

                    if (parameters.Length != 1) return false;

                    var parameter = parameters[0];

                    return parameter.ParameterType.IsGenericType
                           && parameter
                               .ParameterType
                               .GetGenericArguments()
                               .Skip(1)
                               .SequenceEquivalent(methodGenericArguments
                                   .Skip(1)
                                   .Select(a => a.ParameterType));
                });

            if (baseMethod is null) throw new MethodNotImplementedException($"{nameof(ICourier)} does not have a method named {nameof(ICourier.Subscribe)}");

            var subscribeMethod = baseMethod.MakeGenericMethod(methodGenericArguments[0].ParameterType);

            subscribeMethod.Invoke(courier, new object[] { handlerDelegate });
        }

        internal static Type CreateCourierHandlerType(this MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            var notificationType = parameters[0].ParameterType;

            Type handlerType;
            switch (parameters.Length)
            {
                case 2:
                    handlerType = typeof(Action<,>).MakeGenericType(notificationType, typeof(CancellationToken));
                    break;
                case 3:
                    handlerType = typeof(Func<,,>).MakeGenericType(notificationType, typeof(CancellationToken), typeof(Task));
                    break;
                default: throw new UnknownMethodException(methodInfo.Name);
            }

            return handlerType;
        }

        public static bool SequenceEquivalent<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer = null)
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
}
