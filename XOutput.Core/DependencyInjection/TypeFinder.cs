using System;
using System.Linq;
using System.Reflection;

namespace XOutput.Core.DependencyInjection
{
    class TypeFinder
    {
        public Type[] GetAllTypes(Func<Assembly, bool> assemblyFilter = null, Func<Type, bool> typeFilter = null)
        {
            if (assemblyFilter != null)
            {
                return GetAllTypes(typeFilter, AppDomain.CurrentDomain.GetAssemblies().Where(assemblyFilter).ToArray());
            }
            return GetAllTypes(typeFilter, AppDomain.CurrentDomain.GetAssemblies().ToArray());
        }

        public Type[] GetAllTypes(Func<Type, bool> typeFilter, params Assembly[] assemblies)
        {
            if (typeFilter != null)
            {
                return assemblies.SelectMany(a => a.GetTypes()).Where(typeFilter).ToArray();
            }
            return assemblies.SelectMany(a => a.GetTypes()).ToArray();
        }
    }
}
