using Microsoft.VisualStudio.TestTools.UnitTesting;
using XOutput.UpdateChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.UpdateChecker.Tests
{
    [TestClass()]
    public class VersionTests
    {
        [DataRow("1.0.0", "1.0.0", VersionCompare.UpToDate)]
        [DataRow("1.0.0", "1.0.0.0", VersionCompare.NeedsUpgrade)]
        [DataRow("1.0", "2.0", VersionCompare.NeedsUpgrade)]
        [DataRow("1.0.1", "1.0.0", VersionCompare.NewRelease)]
        [DataRow("1.0.1", "1.0.0as", VersionCompare.Error)]
        [DataTestMethod]
        public void CompareTest(string appVersion, string latestVersion, VersionCompare expected)
        {
            Assert.AreEqual(expected, Version.Compare(appVersion, latestVersion));
        }
    }
}