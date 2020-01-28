using System;
using System.Runtime.Serialization;

namespace MediatR.Courier.Exceptions
{
    [Serializable]
    public class MethodNotImplementedException : Exception
    {
        public MethodNotImplementedException()
        {
        }

        public MethodNotImplementedException(string message) : base(message)
        {
        }

        public MethodNotImplementedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MethodNotImplementedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
