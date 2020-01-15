using System;

namespace XOutput.Core.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    public sealed class ResolverMethod : Attribute
    {
        public Scope Scope { get; private set; }
        public ResolverMethod(Scope scope = Scope.Singleton)
        {
            Scope = scope;
        }
    }
}
