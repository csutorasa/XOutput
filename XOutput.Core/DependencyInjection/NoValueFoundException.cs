using System;
using System.Runtime.Serialization;

namespace XOutput.Core.DependencyInjection
{
    [Serializable]
    public sealed class NoValueFoundException : Exception
    {
        public NoValueFoundException() { }

        public NoValueFoundException(string message) : base(message) { }

        public NoValueFoundException(string message, Exception innerException) : base(message, innerException) { }

        public NoValueFoundException(Type type) : this($"No value found for {type.FullName}") { }

        private NoValueFoundException(SerializationInfo info, StreamingContext context)
        {

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
