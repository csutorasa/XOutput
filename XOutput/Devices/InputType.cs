using Newtonsoft.Json;
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

        public static bool operator ==(InputType a, InputType b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            if (ReferenceEquals(a, null))
            {
                return false;
            }
            if (ReferenceEquals(b, null))
            {
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(InputType a, InputType b)
        {
            return !(a == b);
        }

        public override bool Equals(object other)
        {
            if (!(other is InputType))
            {
                return false;
            }
            return Equals((InputType)other);
        }

        public override int GetHashCode()
        {
            return (int)Type ^ Count;
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
            else if (IsOther())
            {
                return "O" + Count;
            }
            return "DISABLED";
        }

        public static InputType Parse(string text)
        {
            if (text.Length > 1 && text != "DISABLED")
            {
                int number;
                if (int.TryParse(text.Substring(1), out number))
                {
                    switch (text[0])
                    {
                        case 'A':
                            return new InputType { Type = InputTypes.Axis, Count = number };
                        case 'B':
                            return new InputType { Type = InputTypes.Button, Count = number };
                        case 'S':
                            return new InputType { Type = InputTypes.Slider, Count = number };
                        case 'O':
                            return new InputType { Type = InputTypes.Other, Count = number };
                        default:
                            break;
                    }
                }
            }
            return new InputType { Type = InputTypes.Disabled };
        }

        public double GetDisableValue()
        {
            if (IsAxis())
            {
                return 0.5;
            }
            return 0;
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
