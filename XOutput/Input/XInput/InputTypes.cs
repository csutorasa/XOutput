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
        private readonly static XInputTypes[] all;
        private readonly static XInputTypes[] buttons;
        private readonly static XInputTypes[] axes;

        static XInputHelper()
        {
            all = (XInputTypes[])Enum.GetValues(typeof(XInputTypes));
            buttons = all.Where(type => type.IsButton()).ToArray();
            axes = all.Where(type => type.IsAxis()).ToArray();
        }

        public static IEnumerable<XInputTypes> Values { get { return all; } }
        public static IEnumerable<XInputTypes> Buttons { get { return buttons; } }
        public static IEnumerable<XInputTypes> Axes { get { return axes; } }
        
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
    }
}
