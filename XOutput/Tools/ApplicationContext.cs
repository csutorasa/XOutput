using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Tools
{
    public class ApplicationContext
    {
        private static readonly ApplicationContext global = new ApplicationContext();
        public static ApplicationContext Global => global;

        private readonly List<Resolver> resolvers = new List<Resolver>();
        public List<Resolver> Resolvers => resolvers;
        private readonly ISet<Type> constructorResolvedTypes = new HashSet<Type>();

        private readonly object lockObj = new object();

        public T Resolve<T>()
        {
            lock (lockObj)
            {
                return (T)Resolve(typeof(T));
            }
        }

        private object Resolve(Type type)
        {
            if (!constructorResolvedTypes.Contains(type))
            {
                Resolvers.AddRange(GetConstructorResolvers(type));
                constructorResolvedTypes.Add(type);
            }
            List<Resolver> currentResolvers = resolvers.Where(r => r.CreatedType.IsAssignableFrom(type)).ToList();
            if (currentResolvers.Count == 0)
            {
                throw new NoValueFoundException(type);
            }
            if (currentResolvers.Count > 1)
            {
                throw new MultipleValuesFoundException(type, currentResolvers);
            }
            Resolver resolver = currentResolvers[0];
            return resolver.Create(resolver.GetDependencies().Select(d => Resolve(d)).ToArray());
        }

        private IEnumerable<Resolver> GetConstructorResolvers(Type type)
        {
            return type.GetConstructors()
                .Where(m => m.GetCustomAttributes(true).OfType<ResolverMethod>().Any())
                .ToDictionary(m => m, m => m.GetCustomAttributes(true).OfType<ResolverMethod>().First())
                .Select(constructor =>
                {
                    Func<object[], object> creator = ((parameters) => constructor.Key.Invoke(parameters));
                    return Resolver.Create(creator, constructor.Key, type, constructor.Value.Scope);
                })
                .ToList();
        }

        public List<T> ResolveAll<T>()
        {
            lock(lockObj) {
                List<Resolver> currentResolvers = resolvers.Where(r => r.CreatedType.IsAssignableFrom(typeof(T))).ToList();
                return resolvers.Select(r => r.Create(r.GetDependencies().Select(d => Resolve(d)).ToArray())).OfType<T>().ToList();
            }
        }

        public ApplicationContext WithResolvers(params Resolver[] tempResolvers)
        {
            ApplicationContext newContext = new ApplicationContext();
            newContext.Resolvers.AddRange(Resolvers);
            newContext.Resolvers.AddRange(tempResolvers);
            return newContext;
        }

        public ApplicationContext WithSingletons(params object[] instances)
        {
            ApplicationContext newContext = new ApplicationContext();
            foreach (var type in constructorResolvedTypes)
            {
                newContext.constructorResolvedTypes.Add(type);
            }
            newContext.Resolvers.AddRange(Resolvers);
            newContext.Resolvers.AddRange(instances.Select(i => Resolver.CreateSingleton(i)));
            return newContext;
        }

        public void AddFromConfiguration(Type type)
        {
            lock (lockObj)
            {
                foreach (var method in type.GetMethods()
                    .Where(m => m.ReturnType != typeof(void))
                    .Where(m => m.IsStatic)
                    .Where(m => m.GetCustomAttributes(true).OfType<ResolverMethod>().Any())
                    .ToDictionary(m => m, m => m.GetCustomAttributes(true).OfType<ResolverMethod>().First()))
                {
                    Func<object[], object> creator = ((parameters) => method.Key.Invoke(null, parameters));
                    var createdType = method.Key.ReturnType;
                    resolvers.Add(Resolver.Create(creator, method.Key, createdType, method.Value.Scope));
                }
            }
        }

        public void Close()
        {
            lock (lockObj)
            {
                foreach (var singleton in resolvers.Where(r => r.IsSingleton).Where(r => typeof(IDisposable).IsAssignableFrom(r.CreatedType)).Select(r => r.Create(new object[0])).OfType<IDisposable>())
                {
                    singleton.Dispose();
                }
                Resolvers.Clear();
            }
        }
    }

    public enum Scope
    {
        Singleton,
        Prototype
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    public sealed class ResolverMethod : Attribute
    {
        public Scope Scope { get; private set; }
        public ResolverMethod(Scope scope = Scope.Singleton)
        {
            Scope = scope;
        }
    }

    public class Resolver
    {
        private readonly Func<object[], object> creator;
        private readonly Type[] dependencies;
        private readonly Type type;
        private readonly Scope scope;
        private object singletonValue;
        public Tools.Scope Scope => scope;
        public bool IsSingleton => scope == Tools.Scope.Singleton;
        public bool IsResolvedSingleton => IsSingleton && singletonValue != null;

        public bool HasDependecies => dependencies.Length > 0;
        public Type CreatedType => type;

        protected Resolver(Func<object[], object> creator, Type[] dependencies, Type type, Scope scope)
        {
            this.creator = creator;
            this.dependencies = dependencies;
            this.type = type;
            this.scope = scope;
        }

        public static Resolver Create(Delegate creator, Scope scope = Scope.Singleton)
        {
            Func<object[], object> func = (args) => creator.DynamicInvoke(args);
            var parameters = creator.Method.GetParameters().Select(p => p.ParameterType).ToArray();
            return new Resolver(func, parameters, creator.Method.ReturnType, scope);
        }

        public static Resolver Create(Func<object[], object>  creator, MethodBase method, Type returnType, Scope scope)
        {
            var parameters = method.GetParameters().Select(p => p.ParameterType).ToArray();
            return new Resolver(creator, parameters, returnType, scope);
        }

        public static Resolver CreateSingleton<T>(T singleton)
        {
            return new Resolver((args) => singleton, new Type[0], singleton.GetType(), Tools.Scope.Singleton);
        }

        internal static Resolver CreateSingleton(object singleton)
        {
            return new Resolver((args) => singleton, new Type[0], singleton.GetType(), Tools.Scope.Singleton);
        }

        public Type[] GetDependencies()
        {
            return dependencies.ToArray();
        }

        public object Create(object[] values)
        {
            if(IsResolvedSingleton)
            {
                return singletonValue;
            }
            object value = creator.Invoke(values);
            if(IsSingleton)
            {
                singletonValue = value;
            }
            return value;
        }

        public override string ToString()
        {
            return (IsSingleton ? "singleton " : "") + type.FullName + ", dependencies: " + string.Join(", ", dependencies.Select(d => d.FullName));
        }
    }

    public class NoValueFoundException : Exception
    {
        public NoValueFoundException() { }

        public NoValueFoundException(string message) : base(message) { }

        public NoValueFoundException(string message, Exception innerException) : base(message, innerException) { }

        public NoValueFoundException(Type type) : this($"No value found for {type.FullName}") { }
    }

    public class MultipleValuesFoundException : Exception
    {
        private readonly List<Resolver> resolvers;
        public List<Resolver> Resolvers => resolvers;
        public MultipleValuesFoundException() { }

        public MultipleValuesFoundException(string message) : base(message) { }

        public MultipleValuesFoundException(string message, Exception innerException) : base(message, innerException) { }
        public MultipleValuesFoundException(Type type, List<Resolver> resolvers) : this($"Multiple values found for {type.FullName}")
        {
            this.resolvers = resolvers;
        }
    }
}
