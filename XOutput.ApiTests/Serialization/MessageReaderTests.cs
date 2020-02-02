using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

/* Unmerged change from project 'XOutput.ApiTests (netcoreapp3.1)'
Before:
using System;
After:
using Moq;
using System;
*/
using
/* Unmerged change from project 'XOutput.ApiTests (netcoreapp3.1)'
Before:
using Moq;
using XOutput.Api.Message;
After:
using XOutput.Api.Message;
*/
XOutput.Api.Message;
using XOutput.Api.Message.Xbox;

namespace XOutput.Api.Serialization.Tests
{
    [TestClass()]
    public class MessageReaderTests
    {
        private MessageReader reader = new MessageReader();

        [TestMethod]
        public void InputDataTest()
        {
            string input = "{\"Type\":\"InputData\",\"Data\":[{\"InputType\":\"test\",\"Value\":0.5}]}";
            var message = reader.ReadMessage(input) as InputDataMessage;
            Assert.IsNotNull(message);
            Assert.AreEqual("InputData", message.Type);
            Assert.IsNotNull(message.Data);
            Assert.IsTrue(message.Data.Count == 1);
            Assert.AreEqual("test", message.Data[0].InputType);
            Assert.AreEqual(0.5, message.Data[0].Value, 0.01);
        }

        [TestMethod]
        public void DebugTest()
        {
            string input = "{\"Type\":\"Debug\",\"Data\":\"test\"}";
            var message = reader.ReadMessage(input) as DebugMessage;
            Assert.IsNotNull(message);
            Assert.AreEqual("Debug", message.Type);
            Assert.AreEqual("test", message.Data);
        }

        [TestMethod]
        public void XboxInputTest()
        {
            string input = "{\"Type\":\"XboxInput\",\"LX\":0.5}";
            var message = reader.ReadMessage(input) as XboxInputMessage;
            Assert.IsNotNull(message);
            Assert.AreEqual("XboxInput", message.Type);
            Assert.AreEqual(0.5, message.LX.Value, 0.001);
            Assert.IsNull(message.LY);
        }

        [TestMethod]
        public void UnknownMessageTest()
        {
            string input = "{\"Type\":\"test\"}";
            var message = reader.ReadMessage(input) as MessageBase;
            Assert.IsNotNull(message);
            Assert.AreEqual("test", message.Type);
        }

        [TestMethod]
        public void UnknownMessageStreamTest()
        {
            string input = "{\"Type\":\"test\"}";
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                var message = reader.ReadMessage(new StreamReader(ms)) as MessageBase;
                Assert.IsNotNull(message);
                Assert.AreEqual("test", message.Type);
            }
        }
    }
}