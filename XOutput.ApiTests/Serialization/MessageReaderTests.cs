using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XOutput.Websocket.Common;
using XOutput.Websocket.Xbox;

namespace XOutput.Serialization.Tests
{
    [TestClass()]
    public class MessageReaderTests
    {
        private static readonly Dictionary<string, Type> deserializationMapping = new Dictionary<string, Type>
            {
                { DebugRequest.MessageType,  typeof(DebugRequest) },
                { XboxInputRequest.MessageType,  typeof(XboxInputRequest) }
            };
        private MessageReader reader = new MessageReader(deserializationMapping);

        [TestMethod]
        public void DebugTest()
        {
            string input = "{\"type\":\"Debug\",\"data\":\"test\"}";
            var message = reader.ReadString(input) as DebugRequest;
            Assert.IsNotNull(message);
            Assert.AreEqual("Debug", message.Type);
            Assert.AreEqual("test", message.Data);
        }

        [TestMethod]
        public void XboxInputTest()
        {
            string input = "{\"type\":\"XboxInput\",\"lx\":0.5}";
            var message = reader.ReadString(input) as XboxInputRequest;
            Assert.IsNotNull(message);
            Assert.AreEqual("XboxInput", message.Type);
            Assert.AreEqual(0.5, message.LX.Value, 0.001);
            Assert.IsNull(message.LY);
        }

        [TestMethod]
        public void UnknownMessageTest()
        {
            string input = "{\"type\":\"test\"}";
            var message = reader.ReadString(input);
            Assert.IsNotNull(message);
            Assert.AreEqual("test", message.Type);
        }

        [TestMethod]
        public void UnknownMessageStreamTest()
        {
            string input = "{\"type\":\"test\"}";
            using MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(input));
            var message = reader.Read(new StreamReader(ms));
            Assert.IsNotNull(message);
            Assert.AreEqual("test", message.Type);
        }
    }
}