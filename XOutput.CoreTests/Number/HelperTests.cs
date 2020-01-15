using Microsoft.VisualStudio.TestTools.UnitTesting;
/* Unmerged change from project 'XOutput.CoreTests (netcoreapp3.1)'
Before:
using System;
After:
using Moq;
using System;
*/

/* Unmerged change from project 'XOutput.CoreTests (netcoreapp3.1)'
Before:
using System.Threading.Tasks;
using Moq;
After:
using System.Threading.Tasks;
*/


namespace XOutput.Core.Number.Tests
{
    [TestClass()]
    public class HelperTests
    {
        [DataRow(5, 5, true)]
        [DataRow(5, 5.0000001, true)]
        [DataRow(5, 5.1, false)]
        [DataRow(4.9, 5, false)]
        [DataTestMethod]
        public void DoubleEquals(double a, double b, bool equals)
        {
            Assert.AreEqual(equals, NumberHelper.DoubleEquals(a, b));
        }
    }
}