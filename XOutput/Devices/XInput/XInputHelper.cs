using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices.XInput
{
    /// <summary>
    /// <see cref="IInputHelper{T}"/> for <see cref="XInputTypes"/>.
    /// </summary>
    public class XInputHelper
    {
        protected static readonly XInputHelper instance = new XInputHelper();
        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        public static XInputHelper Instance => instance;
        /// <summary>
        /// Gets the all values from the enum.
        /// </summary>
        public IEnumerable<XInputTypes> Values => values;
        /// <summary>
        /// Gets all button values.
        /// </summary>
        public IEnumerable<XInputTypes> Buttons => buttons;
        /// <summary>
        /// Gets all axis values.
        /// </summary>
        public IEnumerable<XInputTypes> Axes => axes;
        /// <summary>
        /// Gets all dpad values.
        /// </summary>
        public IEnumerable<XInputTypes> DPad => dPad;

        private readonly IEnumerable<XInputTypes> values;
        private readonly IEnumerable<XInputTypes> buttons;
        private readonly IEnumerable<XInputTypes> axes;
        private readonly IEnumerable<XInputTypes> dPad;

        public XInputHelper()
        {
            values = Enum.GetValues(typeof(XInputTypes)).OfType<XInputTypes>().Distinct().ToArray();
            buttons = values.Where(v => IsButton(v)).ToArray();
            axes = values.Where(v => IsAxis(v)).ToArray();
            dPad = values.Where(v => IsDPad(v)).ToArray();
        }

        public XOutputSource[] GenerateSources()
        {
            var buttonSources = buttons.Select(b => new XOutputSource(b.ToString(), b));
            var axisSources = axes.Select(a => new XOutputSource(a.ToString(), a));
            var dpadSources = dPad.Select(d => new XOutputSource(d.ToString(), d));
            return buttonSources.Concat(axisSources).Concat(dpadSources).ToArray();
        }

        /// <summary>
        /// Gets if the value is axis type.
        /// <para>Implements <see cref="IInputHelper{T}.IsAxis(T)"/> enum value</para>
        /// </summary>
        /// <param name="type"><see cref="XInputTypes"/> enum value</param>
        /// <returns></returns>
        public bool IsAxis(XInputTypes input)
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

        /// <summary>
        /// Gets if the value is button type.
        /// <para>Implements <see cref="IInputHelper{T}.IsButton(T)"/> enum value</para>
        /// </summary>
        /// <param name="type"><see cref="XInputTypes"/> enum value</param>
        /// <returns></returns>
        public bool IsButton(XInputTypes input)
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

        /// <summary>
        /// Gets if the value is DPad type.
        /// <para>Implements <see cref="IInputHelper{T}.IsDPad(T)"/> enum value</para>
        /// </summary>
        /// <param name="type"><see cref="XInputTypes"/> enum value</param>
        /// <returns></returns>
        public bool IsDPad(XInputTypes input)
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

        /// <summary>
        /// Gets the value for disabled mapping.
        /// </summary>
        /// <param name="input"><see cref="XInputTypes"/> enum value</param>
        /// <returns></returns>
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

    /// <summary>
    /// Extension helper class for <see cref="XInputTypes"/>.
    /// It proxies all calls to <see cref="XInputHelper.Instance"/>.
    /// </summary>
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

        public static bool IsButton(this XInputTypes input)
        {
            return XInputHelper.Instance.IsButton(input);
        }

        public static double GetDisableValue(this XInputTypes input)
        {
            return XInputHelper.Instance.GetDisableValue(input);
        }

        public static InputSourceTypes GetInputSourceType(this XInputTypes input)
        {
            if (input.IsAxis())
            {
                return InputSourceTypes.Axis;
            }
            else if (input.IsButton())
            {
                return InputSourceTypes.Button;
            }
            else if (input.IsDPad())
            {
                return InputSourceTypes.Dpad;
            }
            throw new NotImplementedException("ERROR");
        }
    }
}
