using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using XOutput.Websocket;
using XOutput.Websocket.Xbox;

namespace XOutput.Serialization.Tests
{
    [TestClass()]
    public class MessageWriterTests
    {
        private readonly MessageWriter writer = new MessageWriter();

        [TestMethod]
        public void ForceFeedbackTest()
        {
            var input = new XboxFeedbackResponse
            {
                SmallForceFeedback = 0,
                BigForceFeedback = 1,
                LedNumber = 1,
            };
            var message = writer.GetString(input);
            Assert.AreEqual("{\"smallForceFeedback\":0,\"bigForceFeedback\":1,\"ledNumber\":1,\"type\":\"XboxFeedback\"}", message);
        }

        [TestMethod]
        public void UnknownMessageStreamTest()
        {
            var message = new MessageBase { Type = "test" };
            using MemoryStream ms = new MemoryStream(1024);
            writer.Write(message, ms);
            int length = (int)ms.Position;
            ms.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[length];
            ms.Read(buffer, 0, length);
            string output = Encoding.UTF8.GetString(buffer);
            Assert.AreEqual("{\"type\":\"test\"}", output);
        }
    }
}