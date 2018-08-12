using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices.XInput
{
    public static class XInputTypes
    {
        public static readonly InputType A = new InputType { Type = InputTypes.Button, Count = 1 };
        public static readonly InputType B = new InputType { Type = InputTypes.Button, Count = 2 };
        public static readonly InputType X = new InputType { Type = InputTypes.Button, Count = 3 };
        public static readonly InputType Y = new InputType { Type = InputTypes.Button, Count = 4 };
        public static readonly InputType L1 = new InputType { Type = InputTypes.Button, Count = 5 };
        public static readonly InputType R1 = new InputType { Type = InputTypes.Button, Count = 6 };
        public static readonly InputType L3 = new InputType { Type = InputTypes.Button, Count = 7 };
        public static readonly InputType R3 = new InputType { Type = InputTypes.Button, Count = 8 };
        public static readonly InputType Start = new InputType { Type = InputTypes.Button, Count = 9 };
        public static readonly InputType Back = new InputType { Type = InputTypes.Button, Count = 10 };
        public static readonly InputType Home = new InputType { Type = InputTypes.Button, Count = 11 };
        public static readonly InputType LX = new InputType { Type = InputTypes.Axis, Count = 1 };
        public static readonly InputType LY = new InputType { Type = InputTypes.Axis, Count = 2 };
        public static readonly InputType RX = new InputType { Type = InputTypes.Axis, Count = 3 };
        public static readonly InputType RY = new InputType { Type = InputTypes.Axis, Count = 4 };
        public static readonly InputType L2 = new InputType { Type = InputTypes.Axis, Count = 5 };
        public static readonly InputType R2 = new InputType { Type = InputTypes.Axis, Count = 6 };
        public static readonly InputType UP = new InputType { Type = InputTypes.Other, Count = 0 };
        public static readonly InputType LEFT = new InputType { Type = InputTypes.Other, Count = 1 };
        public static readonly InputType DOWN = new InputType { Type = InputTypes.Other, Count = 2 };
        public static readonly InputType RIGHT = new InputType { Type = InputTypes.Other, Count = 3 };
        private static readonly IEnumerable<InputType> values;
        public static IEnumerable<InputType> Values => values;
        private static readonly IEnumerable<InputType> buttons;
        public static IEnumerable<InputType> Buttons => buttons;
        private static readonly IEnumerable<InputType> axes;
        public static IEnumerable<InputType> Axes => axes;

        static XInputTypes()
        {
            values = new InputType[] {
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
            };
            buttons = values.Where(v => v.Type == InputTypes.Button).ToArray();
            axes = values.Where(v => v.Type == InputTypes.Axis).ToArray();
        }
    }
}