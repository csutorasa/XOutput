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
    /// Input enum helper class with some default implementation.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    public abstract class AbstractInputHelper<T> : IInputHelper<T> where T : struct, IConvertible
    {
        public IEnumerable<T> Values => values;
        public IEnumerable<T> Buttons => buttons;
        public IEnumerable<T> Axes => axes;
        public IEnumerable<T> DPad => dPad;
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
            values = (T[])Enum.GetValues(typeof(T));
            buttons = values.Where(v => IsButton(v)).ToArray();
            axes = values.Where(v => IsAxis(v)).ToArray();
            dPad = values.Where(v => IsDPad(v)).ToArray();
            sliders = values.Where(v => IsSlider(v)).ToArray();
        }
        public abstract bool IsAxis(T type);
        public abstract bool IsButton(T type);
        public abstract bool IsDPad(T type);
        public abstract bool IsSlider(T type);
    }
}
