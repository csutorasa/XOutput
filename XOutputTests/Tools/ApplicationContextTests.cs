using Microsoft.VisualStudio.TestTools.UnitTesting;
using XOutput.UpdateChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace XOutput.Tools.Tests
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
            context.Resolvers.Add(Resolver.Create(new Func<object>(() => new object())));
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
        public void MergedDependencyTest()
        {
            ApplicationContext firstContext = new ApplicationContext();
            firstContext.Resolvers.Add(Resolver.Create(new Func<int, double>((a) => 10.1)));
            ApplicationContext context = firstContext.WithResolvers(Resolver.Create(new Func<int>(() => 5)));
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
        public void ConfiguartionTest()
        {
            ApplicationContext context = new ApplicationContext();
            context.AddFromConfiguration(typeof(Configuration));
            int value = context.Resolve<int>();
            Assert.AreEqual(5, value);
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
}