using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XOutput.Devices
{
    /// <summary>
    /// Input enum helper class with some default implementation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    public abstract class AbstractInputHelper<T> : IInputHelper<T> where T : struct, IConvertible
    {
        /// <summary>
        /// Gets all enum values.
        /// </summary>
        public IEnumerable<T> Values => values;
        /// <summary>
        /// Gets all button values.
        /// </summary>
        public IEnumerable<T> Buttons => buttons;
        /// <summary>
        /// Gets all axis values.
        /// </summary>
        public IEnumerable<T> Axes => axes;
        /// <summary>
        /// Gets all dpad values.
        /// </summary>
        public IEnumerable<T> DPad => dPad;
        /// <summary>
        /// Gets all slider values.
        /// </summary>
        public IEnumerable<T> Sliders => sliders;

        private readonly IEnumerable<T> values;
        private readonly IEnumerable<T> buttons;
        private readonly IEnumerable<T> axes;
        private readonly IEnumerable<T> dPad;
        private readonly IEnumerable<T> sliders;

        public AbstractInputHelper()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Type must be enum", nameof(T));
            }
            values = ((T[])Enum.GetValues(typeof(T))).Distinct().ToArray();
            buttons = values.Where(v => IsButton(v)).ToArray();
            axes = values.Where(v => IsAxis(v)).ToArray();
            dPad = values.Where(v => IsDPad(v)).ToArray();
            sliders = values.Where(v => IsSlider(v)).ToArray();
        }

        /// <summary>
        /// Gets if the value is axis type.
        /// </summary>
        /// <param name="type">enum value</param>
        /// <returns></returns>
        public abstract bool IsAxis(T type);
        /// <summary>
        /// Gets if the value is button type.
        /// </summary>
        /// <param name="type">enum value</param>
        /// <returns></returns>
        public abstract bool IsButton(T type);
        /// <summary>
        /// Gets if the value is dpad type.
        /// </summary>
        /// <param name="type">enum value</param>
        /// <returns></returns>
        public abstract bool IsDPad(T type);
        /// <summary>
        /// Gets if the value is slider type.
        /// </summary>
        /// <param name="type">enum value</param>
        /// <returns></returns>
        public abstract bool IsSlider(T type);
    }
}
