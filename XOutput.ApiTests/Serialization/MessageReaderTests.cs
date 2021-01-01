using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XOutput.Api.Message;
using XOutput.Api.Message.Xbox;

namespace XOutput.Api.Serialization.Tests
{
    [TestClass()]
    public class MessageReaderTests
    {
        private static readonly Dictionary<string, Type> deserializationMapping = new Dictionary<string, Type>
            {
                { InputDataMessage.MessageType, typeof(InputDataMessage) },
                { DebugMessage.MessageType,  typeof(DebugMessage) },
                { XboxInputMessage.MessageType,  typeof(XboxInputMessage) }
            };
        private MessageReader reader = new MessageReader(deserializationMapping);

        [TestMethod]
        public void InputDataTest()
        {
            string input = "{\"Type\":\"InputData\",\"Data\":[{\"InputType\":\"test\",\"Value\":0.5}]}";
            var message = reader.ReadString(input) as InputDataMessage;
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
            var message = reader.ReadString(input) as DebugMessage;
            Assert.IsNotNull(message);
            Assert.AreEqual("Debug", message.Type);
            Assert.AreEqual("test", message.Data);
        }

        [TestMethod]
        public void XboxInputTest()
        {
            string input = "{\"Type\":\"XboxInput\",\"LX\":0.5}";
            var message = reader.ReadString(input) as XboxInputMessage;
            Assert.IsNotNull(message);
            Assert.AreEqual("XboxInput", message.Type);
            Assert.AreEqual(0.5, message.LX.Value, 0.001);
            Assert.IsNull(message.LY);
        }

        [TestMethod]
        public void UnknownMessageTest()
        {
            string input = "{\"Type\":\"test\"}";
            var message = reader.ReadString(input);
            Assert.IsNotNull(message);
            Assert.AreEqual("test", message.Type);
        }

        [TestMethod]
        public void UnknownMessageStreamTest()
        {
            string input = "{\"Type\":\"test\"}";
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            {
                var message = reader.Read(new StreamReader(ms));
                Assert.IsNotNull(message);
                Assert.AreEqual("test", message.Type);
            }
        }
    }
}