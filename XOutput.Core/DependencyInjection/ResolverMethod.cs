using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
