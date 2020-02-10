using System;

namespace XOutput.Core.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public sealed class Dependency : Attribute
    {
        public bool Required { get; private set; }
        public Dependency(bool required = true)
        {
            this.Required = required;
        }
    }

    public class DependencyDefinition
    {
        public Type Type { get; set; }
        public bool Required { get; set; }
    }
}
