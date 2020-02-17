using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace XOutput.Devices.Mapper.Tests
{
    [TestClass]
    public class InputMapperCollectionTests
    {
        [DataRow(new double[] { 0 }, 0, 0)]
        [DataRow(new double[] { 0, 0 }, 0, 0)]
        [DataRow(new double[] { 0, 1 }, 0, 1)]
        [DataRow(new double[] { 0.5, 0.5 }, 0.5, 0.5)]
        [DataRow(new double[] { 0, 0.5 }, 0.5, 0)]
        [DataRow(new double[] { 0 }, 0.5, 0)]
        [DataRow(new double[] { 1 }, 0.25, 1)]
        [DataTestMethod]
        public void MapperTest(double[] values, double centerValue, double mappedValue)
        {
            var mapper = new InputMapperCollection
            {
                CenterPoint = centerValue,
            };
            double result = mapper.GetValue(values);
            Assert.AreEqual(mappedValue, result);
        }
    }
}