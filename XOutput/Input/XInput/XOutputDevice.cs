using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;

namespace XOutput.Input.XInput
{
    /// <summary>
    /// Device that contains data for a XInput device
    /// </summary>
    public sealed class XOutputDevice : IDevice
    {
        /// <summary>
        /// This event is invoked if the data from the device was updated
        /// </summary>
        public event Action InputChanged;

        public IEnumerable<DPadDirection> DPads => dPads;
        public IEnumerable<Enum> Buttons => XInputHelper.Instance.Buttons.OfType<Enum>();
        public IEnumerable<Enum> Axes => XInputHelper.Instance.Axes.OfType<Enum>();
        public IEnumerable<Enum> Sliders => new Enum[0];

        private readonly Dictionary<XInputTypes, double> values = new Dictionary<XInputTypes, double>();
        private readonly IInputDevice source;
        private readonly InputMapperBase mapper;
        private DPadDirection[] dPads = new DPadDirection[1];

        /// <summary>
        /// Creates a new XDevice.
        /// </summary>
        /// <param name="source">Direct input device</param>
        /// <param name="mapper">DirectInput to XInput mapper</param>
        public XOutputDevice(IInputDevice source, Mapper.InputMapperBase mapper)
        {
            this.source = source;
            this.mapper = mapper;
            source.InputChanged += Source_InputChanged;
        }

        ~XOutputDevice()
        {
            Dispose();
        }

        public void Dispose()
        {
            source.InputChanged -= Source_InputChanged;
        }

        /// <summary>
        /// Gets the current state of the inputTpye.
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        public double Get(XInputTypes inputType)
        {
            if (values.ContainsKey(inputType))
            {
                return values[inputType];
            }
            return 0;
        }

        private void Source_InputChanged()
        {
            RefreshInput();
        }

        /// <summary>
        /// Refreshes the current state.
        /// </summary>
        /// <returns></returns>
        public bool RefreshInput()
        {
            foreach (var type in XInputHelper.Instance.Values)
            {
                var mapping = mapper.GetMapping(type);
                if (mapping != null)
                {
                    double value = 0;
                    if (mapping.InputType != null)
                        value = source.Get(mapping.InputType);
                    values[type] = mapping.GetValue(value);
                }
            }
            if (source.DPads.Count() > 0)
            {
                dPads[0] = source.DPads.ElementAt(0);
            }
            else
            {
                dPads[0] = DPadHelper.GetDirection(GetBool(XInputTypes.UP), GetBool(XInputTypes.DOWN), GetBool(XInputTypes.LEFT), GetBool(XInputTypes.RIGHT));
            }
            InputChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Gets a snapshot of data.
        /// </summary>
        /// <returns></returns>
        public Dictionary<XInputTypes, double> GetValues()
        {
            var newValues = new Dictionary<XInputTypes, double>(values);
            newValues[XInputTypes.UP] = dPads[0].HasFlag(DPadDirection.Up) ? 1 : 0;
            newValues[XInputTypes.LEFT] = dPads[0].HasFlag(DPadDirection.Left) ? 1 : 0;
            newValues[XInputTypes.RIGHT] = dPads[0].HasFlag(DPadDirection.Right) ? 1 : 0;
            newValues[XInputTypes.DOWN] = dPads[0].HasFlag(DPadDirection.Down) ? 1 : 0;
            return newValues;
        }

        public bool GetBool(XInputTypes inputType)
        {
            return Get(inputType) > 0.5;
        }

        public double Get(Enum inputType)
        {
            if (inputType is XInputTypes)
                return Get((XInputTypes)inputType);
            throw new ArgumentException();
        }
    }
}
