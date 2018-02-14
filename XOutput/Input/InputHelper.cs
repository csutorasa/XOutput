using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XOutput.Input.DirectInput;

namespace XOutput.Input
{
    public interface InputHelper<T> where T : struct, IConvertible
    {
        IEnumerable<T> Values { get; }
        IEnumerable<T> Buttons { get; }
        IEnumerable<T> Axes { get; }
        IEnumerable<T> DPad { get; }

        bool IsButton(T type);
        bool IsAxis(T type);
        bool IsDPad(T type);
        bool IsSlider(T type);
    }

    public abstract class AbstractInputHelper<T> : InputHelper<T> where T : struct, IConvertible
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
            if(!typeof(T).IsEnum)
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
