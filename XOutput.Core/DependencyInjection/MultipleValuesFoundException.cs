using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XOutput.Core.DependencyInjection
{
    [Serializable]
    public sealed class MultipleValuesFoundException : Exception, ISerializable
    {
        private readonly List<Resolver> resolvers;
        public List<Resolver> Resolvers => resolvers;
        private MultipleValuesFoundException() { }

        private MultipleValuesFoundException(string message) : base(message) { }

        private MultipleValuesFoundException(string message, Exception innerException) : base(message, innerException) { }

        public MultipleValuesFoundException(Type type, List<Resolver> resolvers) : this($"Multiple values found for {type.FullName}")
        {
            this.resolvers = resolvers;
        }

        private MultipleValuesFoundException(SerializationInfo info, StreamingContext context)
        {

        }
    }
}
