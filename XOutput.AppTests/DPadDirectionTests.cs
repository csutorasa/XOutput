using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XOutput.Devices.Tests
{
    [TestClass]
    public class DPadDirectionTests
    {
        [DataRow(false, false, false, false, DPadDirection.None)]
        [DataRow(true, false, false, false, DPadDirection.Up)]
        [DataRow(false, true, false, false, DPadDirection.Down)]
        [DataRow(false, false, true, false, DPadDirection.Left)]
        [DataRow(false, false, false, true, DPadDirection.Right)]
        [DataRow(true, false, true, false, DPadDirection.Up | DPadDirection.Left)]
        [DataTestMethod]
        public void GetDirectionTest(bool up, bool down, bool left, bool right, DPadDirection direction)
        {
            DPadDirection dPad = DPadHelper.GetDirection(up, down, left, right);
            Assert.AreEqual(direction, dPad);
        }
    }
}