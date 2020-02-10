using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace XOutput.Core.DependencyInjection.Tests
{
    [TestClass()]
    public class ApplicationContextTests
    {
        [TestMethod]
        public void SimpleTest()
        {
            int expectedValue = 5;
            ApplicationContext context = new ApplicationContext();
            context.Resolvers.Add(Resolver.CreateSingleton(expectedValue));
            int actualValue = context.Resolve<int>();
            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void SingletonTest()
        {
            ApplicationContext context = new ApplicationContext();
            context.Resolvers.Add(Resolver.CreateSingleton(new object()));
            object first = context.Resolve<object>();
            object second = context.Resolve<object>();
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void PrototypeTest()
        {
            ApplicationContext context = new ApplicationContext();
            context.Resolvers.Add(Resolver.Create(new Func<object>(() => new object()), Scope.Prototype));
            object first = context.Resolve<object>();
            object second = context.Resolve<object>();
            Assert.AreNotSame(first, second);
        }

        [TestMethod]
        public void DependencyTest()
        {
            ApplicationContext context = new ApplicationContext();
            context.Resolvers.Add(Resolver.Create(new Func<int, double>((a) => 10.1)));
            context.Resolvers.Add(Resolver.Create(new Func<int>(() => 5)));
            double value = context.Resolve<double>();
            Assert.AreEqual(10.1, value);
        }

        [TestMethod]
        public void ResolveAllTest()
        {
            ApplicationContext context = new ApplicationContext();
            context.Resolvers.Add(Resolver.Create(new Func<int>(() => 10)));
            context.Resolvers.Add(Resolver.Create(new Func<int>(() => 5)));
            List<int> values = context.ResolveAll<int>();
            Assert.IsTrue(values.Contains(10));
            Assert.IsTrue(values.Contains(5));
        }

        [TestMethod]
        public void ResolveAllSubclassTest()
        {
            ApplicationContext context = new ApplicationContext();
            context.Resolvers.Add(Resolver.Create(new Func<A>(() => new A())));
            context.Resolvers.Add(Resolver.Create(new Func<B>(() => new B())));
            context.Resolvers.Add(Resolver.Create(new Func<C>(() => new C())));
            List<A> aValues = context.ResolveAll<A>();
            List<B> bValues = context.ResolveAll<B>();
            List<I> iValues = context.ResolveAll<I>();
            Assert.AreEqual(2, aValues.Count);
            Assert.AreEqual(1, bValues.Count);
            Assert.AreEqual(3, iValues.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(NoValueFoundException))]
        public void NoValueFoundTest()
        {
            ApplicationContext context = new ApplicationContext();
            context.Resolve<int>();
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleValuesFoundException))]
        public void MultipleValuesFoundTest()
        {
            ApplicationContext context = new ApplicationContext();
            context.Resolvers.Add(Resolver.Create(new Func<int>(() => 10)));
            context.Resolvers.Add(Resolver.Create(new Func<int>(() => 10)));
            context.Resolve<int>();
        }

        [TestMethod]
        public void MergedDependencyResolversTest()
        {
            ApplicationContext firstContext = new ApplicationContext();
            firstContext.Resolvers.Add(Resolver.Create(new Func<int, double>((a) => 10.1)));
            ApplicationContext context = firstContext.WithResolvers(Resolver.Create(new Func<int>(() => 5)));
            double value = context.Resolve<double>();
            Assert.AreEqual(10.1, value);
        }

        [TestMethod]
        public void MergedDependencySingletonsTest()
        {
            ApplicationContext firstContext = new ApplicationContext();
            firstContext.Resolvers.Add(Resolver.Create(new Func<int, double>((a) => 10.1)));
            ApplicationContext context = firstContext.WithSingletons(5);
            double value = context.Resolve<double>();
            Assert.AreEqual(10.1, value);
        }

        [TestMethod]
        public void CloseDependencyTest()
        {
            var mock = new Mock<IDisposable>();
            mock.Setup(p => p.Dispose());
            ApplicationContext context = new ApplicationContext();
            context.Resolvers.Add(Resolver.CreateSingleton(mock.Object));
            IDisposable value = context.Resolve<IDisposable>();
            Assert.AreEqual(mock.Object, value);
            mock.Verify(p => p.Dispose(), Times.Never);
            context.Close();
            mock.Verify(p => p.Dispose(), Times.Once);
        }

        [TestMethod]
        public void ConfigurationTest()
        {
            ApplicationContext context = new ApplicationContext();
            context.AddFromConfiguration(typeof(Configuration));
            int value = context.Resolve<int>();
            Assert.AreEqual(5, value);
        }

        [TestMethod]
        public void DiscoverTest()
        {
            ApplicationContext context = new ApplicationContext();
            context.Discover();
            var value = context.Resolve<DiscoveryTest>();
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void NotRequiredValueDependencyTest()
        {
            ApplicationContext context = new ApplicationContext();
            double value = context.Resolve<double>(false);
            Assert.AreEqual(0.0, value, 0.001);
        }

        [TestMethod]
        public void NotRequiredReferenceDependencyTest()
        {
            ApplicationContext context = new ApplicationContext();
            var value = context.Resolve<ApplicationContextTests>(false);
            Assert.IsNull(value);
        }

        [TestMethod]
        public void NotRequiredAttributeTest()
        {
            ApplicationContext context = new ApplicationContext();
            var value = context.Resolve<NotRequiredTest>();
            Assert.IsInstanceOfType(value, typeof(NotRequiredTest));
        }
    }

    static class Configuration
    {
        [ResolverMethod]
        public static int GetInt()
        {
            return 5;
        }
    }

    interface I
    {

    }

    class A : I
    {

    }

    class B : A
    {

    }

    class C : I
    {

    }

    class DiscoveryTest
    {
        [ResolverMethod]
        public DiscoveryTest() { }
    }

    class NotRequiredTest {
        [ResolverMethod]
        public NotRequiredTest([Dependency(false)] int a) { }
    }
}