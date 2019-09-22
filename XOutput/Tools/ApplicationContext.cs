using System;
using System.Collections.Generic;
using System.Linq;
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
                throw new Exception("No value found");
            }
            if (currentResolvers.Count > 1)
            {
                throw new Exception("Multiple values found");
            }
            Resolver resolver = currentResolvers[0];
            return resolver.Create(resolver.GetDependencies().Select(d => Resolve(d)).ToArray());
        }

        public List<T> ResolveAll<T>()
        {
            List<Resolver> currentResolvers = resolvers.Where(r => r.CreatedType.IsAssignableFrom(typeof(T))).ToList();
            return resolvers.Select(r => r.Create(r.GetDependencies().Select(d => Resolve(d)).ToArray())).OfType<T>().ToList();
        }

        public void Close()
        {
            foreach(var singleton in resolvers.Where(r => r.IsSingleton).Where(r => r.CreatedType.IsAssignableFrom(typeof(IDisposable))).Select(r => r.Create(new object[0])).OfType<IDisposable>())
            {
                singleton.Dispose();
            }
            Resolvers.Clear();
        }
    }

    public static class ApplicationContextHelper
    {
        public static ApplicationContext Merge(this ApplicationContext a, ApplicationContext b)
        {
            return Merge(a, b.Resolvers);
        }

        public static ApplicationContext Merge(this ApplicationContext context, List<Resolver> resolvers)
        {
            ApplicationContext newContext = new ApplicationContext();
            newContext.Resolvers.AddRange(context.Resolvers);
            newContext.Resolvers.AddRange(resolvers);
            return newContext;
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
    }
}
