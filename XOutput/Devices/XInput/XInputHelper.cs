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
    public class XInputHelper : AbstractInputHelper<XInputTypes>
    {
        protected static readonly XInputHelper instance = new XInputHelper();
        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        public static XInputHelper Instance => instance;

        /// <summary>
        /// Gets if the value is axis type.
        /// <para>Implements <see cref="IInputHelper{T}.IsAxis(T)"/> enum value</para>
        /// </summary>
        /// <param name="type"><see cref="XInputTypes"/> enum value</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets if the value is button type.
        /// <para>Implements <see cref="IInputHelper{T}.IsButton(T)"/> enum value</para>
        /// </summary>
        /// <param name="type"><see cref="XInputTypes"/> enum value</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets if the value is DPad type.
        /// <para>Implements <see cref="IInputHelper{T}.IsDPad(T)"/> enum value</para>
        /// </summary>
        /// <param name="type"><see cref="XInputTypes"/> enum value</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns false. XInput devices has no sliders.
        /// <para>Implements <see cref="IInputHelper{T}.IsSlider(T)"/> enum value</para>
        /// </summary>
        /// <param name="type"><see cref="XInputTypes"/> enum value</param>
        /// <returns></returns>
        public override bool IsSlider(XInputTypes type)
        {
            return false;
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
    }
}
