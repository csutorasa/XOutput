using System;
using XOutput.Devices;

namespace XOutput.Mapping.Controller.Ds4
{
    public enum Ds4InputTypes
    {
        Circle,
        Cross,
        Triangle,
        Square,
        L1,
        L3,
        R1,
        R3,
        Options,
        Share,
        Ps,
        Up,
        Down,
        Left,
        Right,
        LX,
        LY,
        RX,
        RY,
        L2,
        R2,
}

    public static class Ds4InputTypesExtension
    {
        public static SourceTypes GetSourceType(this Ds4InputTypes input)
        {
            switch (input)
            {
                case Ds4InputTypes.Circle:
                case Ds4InputTypes.Cross:
                case Ds4InputTypes.Triangle:
                case Ds4InputTypes.Square:
                case Ds4InputTypes.L1:
                case Ds4InputTypes.L3:
                case Ds4InputTypes.R1:
                case Ds4InputTypes.R3:
                case Ds4InputTypes.Options:
                case Ds4InputTypes.Share:
                case Ds4InputTypes.Ps:
                    return SourceTypes.Button;
                case Ds4InputTypes.Up:
                case Ds4InputTypes.Down:
                case Ds4InputTypes.Left:
                case Ds4InputTypes.Right:
                    return SourceTypes.Dpad;
                case Ds4InputTypes.LX:
                case Ds4InputTypes.RX:
                    return SourceTypes.AxisX;
                case Ds4InputTypes.LY:
                case Ds4InputTypes.RY:
                    return SourceTypes.AxisY;
                case Ds4InputTypes.L2:
                case Ds4InputTypes.R2:
                    return SourceTypes.Slider;
                default:
                    throw new ArgumentException(nameof(input));
            }
        }

        public static double GetDefaultValue(this Ds4InputTypes input)
        {
            switch (input)
            {
                case Ds4InputTypes.Circle:
                case Ds4InputTypes.Cross:
                case Ds4InputTypes.Triangle:
                case Ds4InputTypes.Square:
                case Ds4InputTypes.L1:
                case Ds4InputTypes.L3:
                case Ds4InputTypes.R1:
                case Ds4InputTypes.R3:
                case Ds4InputTypes.Options:
                case Ds4InputTypes.Share:
                case Ds4InputTypes.Ps:
                case Ds4InputTypes.Up:
                case Ds4InputTypes.Down:
                case Ds4InputTypes.Left:
                case Ds4InputTypes.Right:
                case Ds4InputTypes.L2:
                case Ds4InputTypes.R2:
                    return 0;
                case Ds4InputTypes.LX:
                case Ds4InputTypes.RX:
                case Ds4InputTypes.LY:
                case Ds4InputTypes.RY:
                    return 0.5;
                default:
                    throw new ArgumentException(nameof(input));
            }
        }
    }
}
