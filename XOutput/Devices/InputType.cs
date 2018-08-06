using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices
{
    /// <summary>
    /// Main interface of input devices.
    /// </summary>
    public class InputType : IComparable<InputType>, IEquatable<InputType>
    {
        public InputTypes Type { get; set; }
        public int Count { get; set; }

        public int CompareTo(InputType other)
        {
            int t = Type.CompareTo(other.Type);
            if (t != 0)
            {
                return t;
            }
            return Count.CompareTo(other.Count);
        }

        public bool Equals(InputType other)
        {
            return Type == other.Type && Count == other.Count;
        }

        public bool IsAxis()
        {
            return Type == InputTypes.Axis;
        }

        public bool IsButton()
        {
            return Type == InputTypes.Button;
        }

        public bool IsSlider()
        {
            return Type == InputTypes.Slider;
        }

        public bool IsOther()
        {
            return Type == InputTypes.Other;
        }

        public bool IsDisabled()
        {
            return Type == InputTypes.Disabled;
        }

        public override string ToString()
        {
            if (IsAxis())
            {
                return "A" + Count;
            }
            else if (IsButton())
            {
                return "B" + Count;
            }
            else if (IsSlider())
            {
                return "S" + Count;
            }
            else if (IsSlider())
            {
                return "O" + Count;
            }
            return "DISABLED";
        }
    }

    public enum InputTypes
    {
        Axis,
        Button,
        Slider,
        Other,
        Disabled,
    }
}
