using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Tools
{
    public class ApplicationContext
    {
        private static ApplicationContext global = new ApplicationContext();
        public static ApplicationContext Global => global;

        private readonly List<Resolver> resolvers = new List<Resolver>();
        public List<Resolver> Resolvers => resolvers;

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        private object Resolve(Type type)
        {
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

        public List<T> ResolveAll<T>()
        {
            List<Resolver> currentResolvers = resolvers.Where(r => r.CreatedType.IsAssignableFrom(typeof(T))).ToList();
            return resolvers.Select(r => r.Create(r.GetDependencies().Select(d => Resolve(d)).ToArray())).OfType<T>().ToList();
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
            newContext.Resolvers.AddRange(Resolvers);
            newContext.Resolvers.AddRange(instances.Select(i => Resolver.CreateSingleton(i)));
            return newContext;
        }

        public void AddFromConfiguration(Type type)
        {
            foreach (var method in type.GetMethods()
                .Where(m => m.ReturnType != typeof(void))
                .Where(m => m.IsStatic)
                .Where(m => m.GetCustomAttributes(true).OfType<ResolverMethod>().Any())
                .ToDictionary(m => m, m => m.GetCustomAttributes(true).OfType<ResolverMethod>().First()))
            {
                Type[] funcTypes = method.Key.GetParameters().Select(p => p.ParameterType).Concat(new[] { method.Key.ReturnType }).ToArray();
                Delegate del = Delegate.CreateDelegate(Expression.GetFuncType(funcTypes), method.Key);
                resolvers.Add(Resolver.Create(del, method.Value.Singleton));
            }
        }

        public void Close()
        {
            foreach(var singleton in resolvers.Where(r => r.IsSingleton).Where(r => typeof(IDisposable).IsAssignableFrom(r.CreatedType)).Select(r => r.Create(new object[0])).OfType<IDisposable>())
            {
                singleton.Dispose();
            }
            Resolvers.Clear();
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ResolverMethod : Attribute
    {
        public bool Singleton { get; private set; }
        public ResolverMethod(bool singleton = true)
        {
            Singleton = singleton;
        }
    }

    public class Resolver
    {
        private Delegate creator;
        private Type[] dependencies;
        private Type type;
        private bool isSingleton;
        private object singletonValue;
        public bool IsSingleton => isSingleton;
        public bool IsResolvedSingleton => isSingleton && singletonValue != null;

        public bool HasDependecies => dependencies.Length > 0;
        public Type CreatedType => type;

        protected Resolver(Delegate creator, Type[] dependencies, Type type, bool isSingleton)
        {
            this.creator = creator;
            this.dependencies = dependencies;
            this.type = type;
            this.isSingleton = isSingleton;
        }

        public static Resolver Create(Delegate creator, bool isSingleton = false)
        {
            var dependencies = creator.Method.GetParameters().Select(pi => pi.ParameterType).ToArray();
            return new Resolver(creator, dependencies, creator.Method.ReturnType, isSingleton);
        }

        public static Resolver CreateSingleton<T>(T singleton)
        {
            return Create(new Func<T>(() => singleton), true);
        }

        internal static Resolver CreateSingleton(object singleton)
        {
            return new Resolver(new Func<object>(() => singleton), new Type[0], singleton.GetType(), true);
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
            object value = creator.DynamicInvoke(values);
            if(IsSingleton)
            {
                singletonValue = value;
            }
            return value;
        }

        public override string ToString()
        {
            return (isSingleton ? "singleton " : "") + type.FullName + ", dependencies: " + string.Join(", ", dependencies.Select(d => d.FullName));
        }
    }

    public class NoValueFoundException : Exception
    {
        public NoValueFoundException(Type type) : base($"No value found for {type.FullName}") { }
    }

    public class MultipleValuesFoundException : Exception
    {
        private List<Resolver> resolvers;
        public List<Resolver> Resolvers => resolvers;
        public MultipleValuesFoundException(Type type, List<Resolver> resolvers) : base($"Multiple values found for {type.FullName}")
        {
            this.resolvers = resolvers;
        }
    }
}
