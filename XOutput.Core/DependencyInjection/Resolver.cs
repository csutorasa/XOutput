using System;
using System.Linq;
using System.Reflection;

namespace XOutput.Core.DependencyInjection
{
    public class Resolver
    {
        private readonly Func<object[], object> creator;
        private readonly Dependency[] dependencies;
        private readonly Type type;
        private readonly Scope scope;
        private object singletonValue;
        public Scope Scope => scope;
        public bool IsSingleton => scope == Scope.Singleton;
        public bool IsResolvedSingleton => IsSingleton && singletonValue != null;

        public bool HasDependecies => dependencies.Length > 0;
        public Type CreatedType => type;

        protected Resolver(Func<object[], object> creator, Dependency[] dependencies, Type type, Scope scope)
        {
            this.creator = creator;
            this.dependencies = dependencies;
            this.type = type;
            this.scope = scope;
        }

        public static Resolver Create(Delegate creator, Scope scope = Scope.Singleton)
        {
            Func<object[], object> func = (args) => creator.DynamicInvoke(args);
            var parameters = creator.Method.GetParameters().Select(p => new Dependency
            {
                Type = p.ParameterType,
                Required = true,
            }).ToArray();
            return new Resolver(func, parameters, creator.Method.ReturnType, scope);
        }

        public static Resolver Create(Func<object[], object> creator, MethodBase method, Type returnType, Scope scope)
        {
            var parameters = method.GetParameters().Select(p => new Dependency
            {
                Type = p.ParameterType,
                Required = p.GetCustomAttributes(typeof(DependencyAttribute))
                    .OfType<DependencyAttribute>()
                    .Select(d => (bool?)d.Required)
                    .FirstOrDefault() ?? true,
            }).ToArray();
            return new Resolver(creator, parameters, returnType, scope);
        }

        public static Resolver CreateSingleton<T>(T singleton)
        {
            return new Resolver((args) => singleton, new Dependency[0], typeof(T), Scope.Singleton);
        }

        internal static Resolver CreateSingleton(object singleton)
        {
            return new Resolver((args) => singleton, new Dependency[0], singleton.GetType(), Scope.Singleton);
        }

        public Dependency[] GetDependencies()
        {
            return dependencies.ToArray();
        }

        public object Create(object[] values)
        {
            if (IsResolvedSingleton)
            {
                return singletonValue;
            }
            object value = creator.Invoke(values);
            if (IsSingleton)
            {
                singletonValue = value;
            }
            return value;
        }

        public override string ToString()
        {
            return (IsSingleton ? "singleton " : "") + type.FullName + ", dependencies: " + string.Join(", ", dependencies.Select(d => d.Type.FullName));
        }
    }
}
