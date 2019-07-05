using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.Input;

namespace XOutput.Devices
{
    /// <summary>
    /// Main class for sources.
    /// </summary>
    public abstract class InputSource
    {
        /// <summary>
        /// This event is invoked if the data from the source was updated.
        /// </summary>
        public event SourceChangedEventHandler InputChanged;
        /// <summary>
        /// The display name of the source.
        /// </summary>
        public string DisplayName => name;
        /// <summary>
        /// The type of the source.
        /// </summary>
        public InputSourceTypes Type => type;
        /// <summary>
        /// The device of the source.
        /// </summary>
        public IInputDevice InputDevice => inputDevice;
        /// <summary>
        /// The offset of the source.
        /// </summary>
        public int Offset => offset;
        /// <summary>
        /// The value of the source.
        /// </summary>
        public double Value => value;
        /// <summary>
        /// If the input is an axis.
        /// </summary>
        public bool IsAxis => InputSourceTypes.Axis.HasFlag(type);
        /// <summary>
        /// If the input is a button.
        /// </summary>
        public bool IsButton => type == InputSourceTypes.Button;

        protected IInputDevice inputDevice;
        protected string name;
        protected InputSourceTypes type;
        protected int offset;
        protected double value;

        public InputSource(IInputDevice inputDevice, string name, InputSourceTypes type, int offset)
        {
            this.inputDevice = inputDevice;
            this.name = name;
            this.type = type;
            this.offset = offset;
        }

        public override string ToString()
        {
            return name;
        }

        protected void InvokeChange()
        {
            InputChanged?.Invoke(this, null);
        }

        protected bool RefreshValue(double newValue)
        {
            if (newValue != value)
            {
                value = newValue;
                InvokeChange();
                return true;
            }
            return false;
        }

        public double Get()
        {
            if (inputDevice != null)
            {
                return inputDevice.Get(this);
            }
            return 0;
        }
    }

    public class DisabledInputSource : InputSource
    {
        public static InputSource Instance => instance;
        private static InputSource instance = new DisabledInputSource();

        private DisabledInputSource() : base(null, "", InputSourceTypes.Disabled, 0)
        {

        }
    }
}
