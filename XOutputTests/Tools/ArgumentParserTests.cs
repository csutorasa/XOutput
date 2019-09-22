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
    public class ArgumentParserTests
    {
        [DataRow(new String[] { }, false)]
        [DataRow(new String[] { "--minimized" }, true)]
        [DataRow(new String[] { "other" }, false)]
        [DataRow(new String[] { "other", "--minimized" }, true)]
        [DataTestMethod]
        public void CompareTest(string[] arguments, bool minimized)
        {
            ArgumentParser parser = new ArgumentParser(arguments);
            Assert.AreEqual(minimized, parser.Minimized);
        }
    }
}