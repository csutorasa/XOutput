using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace XOutput.Core.Exceptions.Tests
{
    [TestClass()]
    public class ExceptionHandlerTests
    {
        ExceptionHandler exceptionHandler = new ExceptionHandler();
        int x = 0;

        [TestMethod]
        public void SafeCallActionErrorTest()
        {
            var result = exceptionHandler.SafeCall(() => throw new AccessViolationException());
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasError);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(typeof(AccessViolationException), result.Error.GetType());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SafeCallActionErrorListTest()
        {
            exceptionHandler.SafeCall(() => throw new Exception(), typeof(AccessViolationException));
        }

        [TestMethod]
        public void SafeCallActionErrorListSubClassTest()
        {
            var result = exceptionHandler.SafeCall(() => throw new AccessViolationException(), typeof(Exception));
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasError);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(typeof(AccessViolationException), result.Error.GetType());
        }

        [TestMethod]
        public void SafeCallFuncSuccessTest()
        {
            var result = exceptionHandler.SafeCall(() => {
                return 1;
            });
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasError);
            Assert.AreEqual(1, result.Result);
        }

        [TestMethod]
        public void SafeCallFuncErrorTest()
        {
            var result = exceptionHandler.SafeCall(() => {
                if (x < 1)
                {
                    throw new AccessViolationException();
                }
                return 1;
            });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasError);
            Assert.IsNotNull(result.Error);
            Assert.AreEqual(typeof(AccessViolationException), result.Error.GetType());
        }
    }
}