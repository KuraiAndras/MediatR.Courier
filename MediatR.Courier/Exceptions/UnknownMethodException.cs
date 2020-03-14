using System;
using System.Runtime.Serialization;

namespace MediatR.Courier.Exceptions
{
    [Serializable]
    public class UnknownMethodException : Exception
    {
        public UnknownMethodException()
        {
        }

        public UnknownMethodException(string message)
            : base(message)
        {
        }

        public UnknownMethodException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UnknownMethodException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}