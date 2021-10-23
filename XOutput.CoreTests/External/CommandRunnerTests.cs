using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace XOutput.External.Tests
{
    [TestClass()]
    public class CommandRunnerTests
    {
        CommandRunner commandRunner = new CommandRunner();

        [TestMethod]
        public void CreateProcessTest()
        {
            Process process = commandRunner.CreatePowershell("echo \"asd\"");
            Assert.AreEqual("powershell", process.StartInfo.FileName);
            Assert.AreEqual("-Command \"echo \\\"asd\\\"\"", process.StartInfo.Arguments);
            Assert.IsTrue(process.StartInfo.CreateNoWindow);
            Assert.IsFalse(process.StartInfo.UseShellExecute);
        }
    }
}