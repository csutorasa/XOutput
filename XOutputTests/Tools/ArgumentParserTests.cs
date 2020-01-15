using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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