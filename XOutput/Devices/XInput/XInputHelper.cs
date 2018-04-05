using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices.XInput
{
    /// <summary>
    /// Key enum helper class.
    /// </summary>
    public class XInputHelper : AbstractInputHelper<XInputTypes>
    {
        public static readonly XInputHelper instance = new XInputHelper();
        public static XInputHelper Instance => instance;

        public override bool IsButton(XInputTypes input)
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

        public override bool IsAxis(XInputTypes input)
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

        public override bool IsDPad(XInputTypes input)
        {
            switch (input)
            {
                case XInputTypes.LEFT:
                case XInputTypes.RIGHT:
                case XInputTypes.UP:
                case XInputTypes.DOWN:
                    return true;
                default:
                    return false;
            }
        }

        public override bool IsSlider(XInputTypes type)
        {
            return false;
        }

        public double GetDisableValue(XInputTypes input)
        {
            switch (input)
            {
                case XInputTypes.LX:
                case XInputTypes.LY:
                case XInputTypes.RX:
                case XInputTypes.RY:
                    return 0.5;
                default:
                    return 0;
            }
        }
    }

    public static class XInputExtension
    {
        public static bool IsAxis(this XInputTypes input)
        {
            return XInputHelper.Instance.IsAxis(input);
        }

        public static bool IsDPad(this XInputTypes input)
        {
            return XInputHelper.Instance.IsDPad(input);
        }

        public static double GetDisableValue(this XInputTypes input)
        {
            return XInputHelper.Instance.GetDisableValue(input);
        }
    }
}
