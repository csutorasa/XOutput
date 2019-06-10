using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// The value of the source.
        /// </summary>
        public double Value => value;
        /// <summary>
        /// If the input is an axis.
        /// </summary>
        public bool IsAxis => type == InputSourceTypes.Axis;
        /// <summary>
        /// If the input is a button.
        /// </summary>
        public bool IsButton => type == InputSourceTypes.Button;

        protected string name;
        protected InputSourceTypes type;
        protected double value;

        public InputSource(string name, InputSourceTypes type)
        {
            this.name = name;
            this.type = type;
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
    }

    public class DisabledInputSource : InputSource
    {
        public static InputSource Instance => instance;
        private static InputSource instance = new DisabledInputSource();

        private DisabledInputSource() : base("", InputSourceTypes.Disabled)
        {

        }
    }
}
