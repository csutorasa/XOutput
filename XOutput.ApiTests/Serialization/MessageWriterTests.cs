using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using XOutput.Api.Message;
using XOutput.Api.Message.Xbox;

namespace XOutput.Api.Serialization.Tests
{
    [TestClass()]
    public class MessageWriterTests
    {
        private readonly MessageWriter writer = new MessageWriter();

        [TestMethod]
        public void ForceFeedbackTest()
        {
            var input = new XboxFeedbackMessage
            {
                Small = 0,
                Large = 1,
                LedNumber = 1,
            };
            var message = writer.GetString(input);
            Assert.AreEqual("{\"Small\":0.0,\"Large\":1.0,\"LedNumber\":1,\"Type\":\"XboxFeedback\"}", message);
        }

        [TestMethod]
        public void UnknownMessageStreamTest()
        {
            var message = new MessageBase { Type = "test" };
            using (MemoryStream ms = new MemoryStream(1024))
            {
                writer.Write(message, ms);
                int length = (int)ms.Position;
                ms.Seek(0, SeekOrigin.Begin);
                byte[] buffer = new byte[length];
                ms.Read(buffer, 0, length);
                string output = Encoding.UTF8.GetString(buffer);
                Assert.AreEqual("{\"Type\":\"test\"}", output);
            }
        }
    }
}