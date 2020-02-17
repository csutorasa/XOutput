using System;
using System.Collections.Generic;
using System.Linq;

namespace XOutput.Devices.Mapper
{
    public class InputMapperCollection
    {
        public List<InputMapper> Mappers { get; set; }

        public double CenterPoint { get; set; }

        public InputMapperCollection(params InputMapper[] data) : this(new List<InputMapper>(data), 0)
        {

        }

        public InputMapperCollection(List<InputMapper> mappers, double centerPoint)
        {
            Mappers = mappers;
            CenterPoint = centerPoint;
        }

        public double GetValue(IEnumerable<double> values)
        {
            return values.Aggregate(CenterPoint, (acc, v) => acc + DiffFromCenter(v));
        }

        private double DiffFromCenter(double value)
        {
            double difference = value - CenterPoint;
            if (Math.Abs(difference) < 0.0001)
            {
                return 0;
            }
            return difference;
        }
    }
}
