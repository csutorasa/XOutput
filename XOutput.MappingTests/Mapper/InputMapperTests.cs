using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XOutput.Mapping.Mapper.Tests
{
    [TestClass]
    public class InputMapperTests
    {
        [DataRow(0, 0, 0, 0)]
        [DataRow(0, 0, 1, 0)]
        [DataRow(0, 1, 0, 0)]
        [DataRow(0, 1, 0.4, 0.4)]
        [DataRow(0, 0.5, 0.25, 0.5)]
        [DataRow(0.25, 0.75, 0.5, 0.5)]
        [DataTestMethod]
        public void MapperTest(double min, double max, double value, double mappedValue)
        {
            var mapper = new InputMapper
            {
                MinValue = min,
                MaxValue = max,
            };
            double result = mapper.GetValue(value);
            Assert.AreEqual(mappedValue, result);
        }
    }
}