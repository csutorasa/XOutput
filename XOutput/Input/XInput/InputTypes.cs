using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input.XInput
{
    public enum XInputTypes
    {
        // Buttons
        A,
        B,
        X,
        Y,
        L1,
        R1,
        L3,
        R3,
        Start,
        Back,
        Home,

        // Axes
        LX,
        LY,
        RX,
        RY,
        L2,
        R2
    }

    public static class XInputHelper
    {
        public static IEnumerable<XInputTypes> GetAll()
        {
            return (XInputTypes[])Enum.GetValues(typeof(XInputTypes));
        }
        public static bool IsButton(this XInputTypes input)
        {
            switch (input)
            {
                case XInputTypes.A:
                case XInputTypes.B:
                case XInputTypes.X:
                case XInputTypes.Y:
                case XInputTypes.L1:
                case XInputTypes.R1:
                case XInputTypes.L3:
                case XInputTypes.R3:
                case XInputTypes.Start:
                case XInputTypes.Back:
                case XInputTypes.Home:
                    return true;
                default:
                    return false;
            }
        }
        public static IEnumerable<XInputTypes> GetButtons()
        {
            XInputTypes[] outputTypes = (XInputTypes[])Enum.GetValues(typeof(XInputTypes));
            return outputTypes.Where(type => type.IsButton());
        }

        public static bool IsAxis(this XInputTypes input)
        {
            switch (input)
            {
                case XInputTypes.LX:
                case XInputTypes.LY:
                case XInputTypes.RX:
                case XInputTypes.RY:
                case XInputTypes.L2:
                case XInputTypes.R2:
                    return true;
                default:
                    return false;
            }
        }
        public static IEnumerable<XInputTypes> GetAxes()
        {
            XInputTypes[] outputTypes = (XInputTypes[])Enum.GetValues(typeof(XInputTypes));
            return outputTypes.Where(type => type.IsAxis());
        }
    }
}
