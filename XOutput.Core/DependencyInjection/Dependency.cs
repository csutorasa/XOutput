using System;

namespace XOutput.Core.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class DependencyAttribute : Attribute
    {
        public bool Required { get; private set; }
        public DependencyAttribute(bool required = true)
        {
            this.Required = required;
        }
    }

    public class Dependency
    {
        public Type Type { get; set; }
        public bool Required { get; set; }
    }
}
