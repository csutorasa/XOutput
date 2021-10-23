using System;

namespace XOutput.Mapping.Controller.Xbox
{
    public enum XboxInputTypes
    {
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
        LX,
        LY,
        RX,
        RY,
        L2,
        R2,
        Up,
        Down,
        Left,
        Right,
    }

    public static class XboxInputTypesExtension
    {
        public static double GetDefaultValue(this XboxInputTypes input)
        {
            switch (input)
            {
                case XboxInputTypes.A:
                case XboxInputTypes.B:
                case XboxInputTypes.X:
                case XboxInputTypes.Y:
                case XboxInputTypes.L1:
                case XboxInputTypes.R1:
                case XboxInputTypes.L3:
                case XboxInputTypes.R3:
                case XboxInputTypes.Start:
                case XboxInputTypes.Back:
                case XboxInputTypes.Home:
                case XboxInputTypes.Up:
                case XboxInputTypes.Down:
                case XboxInputTypes.Left:
                case XboxInputTypes.Right:
                case XboxInputTypes.L2:
                case XboxInputTypes.R2:
                    return 0;
                case XboxInputTypes.LX:
                case XboxInputTypes.RX:
                case XboxInputTypes.LY:
                case XboxInputTypes.RY:
                    return 0.5;
                default:
                    throw new ArgumentException(nameof(input));
            }
        }
    }
}
