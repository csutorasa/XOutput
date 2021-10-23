using System;
using System.Collections;

namespace XOutput.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class DependencyAttribute : Attribute
    {
        public bool Required { get; private set; }
        public DependencyAttribute(bool required = true)
        {
            Required = required;
        }
    }

    public class Dependency
    {
        public Type Type { get; set; }
        public bool Required { get; set; }
        public bool IsEnumerable => typeof(IEnumerable).IsAssignableFrom(Type) && Type.GenericTypeArguments.Length == 1;
    }
}
