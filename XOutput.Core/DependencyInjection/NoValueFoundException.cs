using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

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
    }
}
