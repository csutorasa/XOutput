using Microsoft.VisualStudio.TestTools.UnitTesting;

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