using System;
using System.Collections.Generic;
using System.Linq;

namespace XOutput.Devices.Mapper
{
    public class InputMapperCollection
    {
        public List<InputMapper> Mappers { get; set; }

        public double CenterPoint
        {
            get => centerPoint;
            set
            {
                if (value != centerPoint)
                {
                    centerPoint = value;
                    lowRange = centerPoint;
                    highRange = 1 - centerPoint;
                }
            }
        }

        private double centerPoint = -1;
        private double lowRange;
        private double highRange;

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
            return values.Aggregate(centerPoint, (acc, v) => acc + DiffFromCenter(v));
        }

        private double DiffFromCenter(double value)
        {
            double difference = value - centerPoint;
            if (Math.Abs(difference) < 0.0001)
            {
                return 0;
            }
            if (value < centerPoint)
            {
                return difference * lowRange;
            }
            return difference * highRange;
        }
    }
}
