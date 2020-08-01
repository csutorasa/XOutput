using System;
using System.Collections.Generic;
using XOutput.Core.Configuration;

namespace XOutput.Devices.Input
{
    public class InputConfig : ConfigurationBase, IEquatable<InputConfig>
    {
        public List<int> BigMotors { get; set; }
        public List<int> SmallMotors { get; set; }

        public InputConfig()
        {
            BigMotors = new List<int>();
            SmallMotors = new List<int>();
        }

        public bool Equals(InputConfig other)
        {
            return Equals(BigMotors, other.BigMotors)
                && Equals(SmallMotors, other.SmallMotors);
        }
    }
}
