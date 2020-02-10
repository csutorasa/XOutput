using System;
using System.Collections.Generic;
using System.Linq;

namespace XOutput.Core.DependencyInjection
{
    public class ApplicationContext
    {
        private static readonly ApplicationContext global = new ApplicationContext();
        public static ApplicationContext Global => global;

        static ApplicationContext()
        {
            global.Resolvers.Add(Resolver.CreateSingleton(global));
        }

        private readonly List<Resolver> resolvers = new List<Resolver>();
        public List<Resolver> Resolvers => resolvers;
        private readonly ISet<Type> constructorResolvedTypes = new HashSet<Type>();
        private readonly TypeFinder typeFinder = new TypeFinder();

        public void Discover()
        {
            lock (lockObj)
            {
                var types = typeFinder.GetAllTypes(a => a.FullName.StartsWith("XOutput"));
                foreach (var type in types)
                {
                    if (!constructorResolvedTypes.Contains(type))
                    {
                        Resolvers.AddRange(GetConstructorResolvers(type));
                        constructorResolvedTypes.Add(type);
                    }
                }
            }
        }

        private readonly object lockObj = new object();

        public T Resolve<T>(bool required = true)
        {
            lock (lockObj)
            {
                return (T)Resolve(new DependencyDefinition {
                    Type = typeof(T),
                    Required = required,
                });
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

        private object Resolve(DependencyDefinition dependency)
        {
            try
            {
                return Resolve(dependency.Type);
            }
            catch (NoValueFoundException)
            {
                if (dependency.Required)
                {
                    throw;
                }
                if (dependency.Type.IsValueType)
                {
                    return Activator.CreateInstance(dependency.Type);
                }
                return null;
            }
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
            lock (lockObj)
            {
                List<Resolver> currentResolvers = resolvers.Where(r => typeof(T).IsAssignableFrom(r.CreatedType)).ToList();
                return currentResolvers.Select(r => r.Create(r.GetDependencies().Select(d => Resolve(d)).ToArray())).OfType<T>().ToList();
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
}
