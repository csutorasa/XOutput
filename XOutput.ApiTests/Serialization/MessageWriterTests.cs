using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using XOutput.Api.Message;

namespace XOutput.Api.Serialization.Tests
{
    [TestClass()]
    public class MessageWriterTests
    {
        private MessageWriter writer = new MessageWriter();

        [TestMethod]
        public void ForceFeedbackTest()
        {
            var input = new ForceFeedbackMessage
            {
                Small = 0,
                Large = 1,
            };
            var message = writer.WriteMessage(input);
            Assert.AreEqual("{\"Small\":0.0,\"Large\":1.0,\"Type\":\"ForceFeedback\"}", message);
        }
    }
}