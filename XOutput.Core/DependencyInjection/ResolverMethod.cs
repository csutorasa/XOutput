using System;

namespace XOutput.Core.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    public sealed class ResolverMethodAttribute : Attribute
    {
        public Scope Scope { get; private set; }
        public ResolverMethodAttribute(Scope scope = Scope.Singleton)
        {
            Scope = scope;
        }
    }
}
