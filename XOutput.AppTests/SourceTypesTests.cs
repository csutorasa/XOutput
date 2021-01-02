using Microsoft.VisualStudio.TestTools.UnitTesting;
using XOutput.App.Devices;

namespace XOutput.Devices.Tests
{
    [TestClass]
    public class InputMapperTests
    {
        [DataRow(SourceTypes.AxisX, true)]
        [DataRow(SourceTypes.AxisY, true)]
        [DataRow(SourceTypes.AxisZ, true)]
        [DataRow(SourceTypes.Axis, true)]
        [DataRow(SourceTypes.Button, false)]
        [DataRow(SourceTypes.None, false)]
        [DataRow(SourceTypes.Dpad, false)]
        [DataRow(SourceTypes.Slider, false)]
        [DataTestMethod]
        public void IsAxisTest(SourceTypes type, bool isAxis)
        {
            Assert.AreEqual(isAxis, type.IsAxis());
        }
    }
}