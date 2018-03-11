using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XOutput.Input.DirectInput;

namespace XOutput.Input
{
    /// <summary>
    /// Input enum helper class.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    public interface IInputHelper<T> where T : struct, IConvertible
    {
        /// <summary>
        /// Gets all values of the enum
        /// </summary>
        IEnumerable<T> Values { get; }
        /// <summary>
        /// Gets all button values of the enum
        /// </summary>
        IEnumerable<T> Buttons { get; }
        /// <summary>
        /// Gets all axis values of the enum
        /// </summary>
        IEnumerable<T> Axes { get; }
        /// <summary>
        /// Gets all DPad values of the enum
        /// </summary>
        IEnumerable<T> DPad { get; }
        /// <summary>
        /// Gets all Slider values of the enum
        /// </summary>
        IEnumerable<T> Sliders { get; }

        /// <summary>
        /// Returns if the given enum value is an axis.
        /// </summary>
        /// <param name="type">enum value</param>
        /// <returns></returns>
        bool IsButton(T type);
        /// <summary>
        /// Returns if the given enum value is a button.
        /// </summary>
        /// <param name="type">enum value</param>
        /// <returns></returns>
        bool IsAxis(T type);
        /// <summary>
        /// Returns if the given enum value is a DPad.
        /// </summary>
        /// <param name="type">enum value</param>
        /// <returns></returns>
        bool IsDPad(T type);
        /// <summary>
        /// Returns if the given enum value is a slider.
        /// </summary>
        /// <param name="type">enum value</param>
        /// <returns></returns>
        bool IsSlider(T type);
    }
}
