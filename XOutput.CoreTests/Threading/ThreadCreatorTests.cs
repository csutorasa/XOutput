using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace XOutput.Core.Threading.Tests
{
    [TestClass()]
    public class ThreadCreatorTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void Create()
        {
            ThreadContext context = ThreadCreator.Create("test", ThreadAction, true);
            ThreadResult result = context.Start().Cancel().Wait();
            Assert.IsNull(result.Error);
        }

        private void ThreadAction(CancellationToken token)
        {
            Assert.AreEqual("test", Thread.CurrentThread.Name);
            Assert.IsTrue(Thread.CurrentThread.IsBackground);
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(0);
            }
        }
    }
}