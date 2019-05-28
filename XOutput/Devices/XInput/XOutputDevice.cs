using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.Input;
using XOutput.Devices.Mapper;

namespace XOutput.Devices.XInput
{
    /// <summary>
    /// Device that contains data for a XInput device
    /// </summary>
    public sealed class XOutputDevice : IDevice
    {
        #region Constants
        /// <summary>
        /// XInput devices has 1 DPad.
        /// </summary>
        public const int DPadCount = 1;
        #endregion

        #region Events
        /// <summary>
        /// This event is invoked if the data from the device was updated
        /// <para>Implements <see cref="IDevice.InputChanged"/></para>
        /// </summary>
        public event DeviceInputChangedHandler InputChanged;
        #endregion

        #region Properties
        /// <summary>
        /// <para>Implements <see cref="IDevice.DPads"/></para>
        /// </summary>
        public IEnumerable<DPadDirection> DPads => dPads;
        /// <summary>
        /// <para>Implements <see cref="IDevice.Buttons"/></para>
        /// </summary>
        public IEnumerable<Enum> Buttons => XInputHelper.Instance.Buttons.OfType<Enum>();
        /// <summary>
        /// <para>Implements <see cref="IDevice.Axes"/></para>
        /// </summary>
        public IEnumerable<Enum> Axes => XInputHelper.Instance.Axes.OfType<Enum>();
        /// <summary>
        /// XInput devices have no sliders.
        /// <para>Implements <see cref="IDevice.Sliders"/></para>
        /// </summary>
        public IEnumerable<Enum> Sliders => new Enum[0];
        #endregion

        private readonly Dictionary<XInputTypes, double> values = new Dictionary<XInputTypes, double>();
        private readonly IInputDevice source;
        private readonly InputMapperBase mapper;
        private readonly DPadDirection[] dPads = new DPadDirection[DPadCount];
        private readonly DeviceState state;

        /// <summary>
        /// Creates a new XDevice.
        /// </summary>
        /// <param name="source">Direct input device</param>
        /// <param name="mapper">DirectInput to XInput mapper</param>
        public XOutputDevice(IInputDevice source, Mapper.InputMapperBase mapper)
        {
            this.source = source;
            this.mapper = mapper;
            state = new DeviceState(XInputHelper.Instance.Values.OfType<Enum>().ToArray(), DPadCount);
            source.InputChanged += SourceInputChanged;
        }

        ~XOutputDevice()
        {
            Dispose();
        }

        public void Dispose()
        {
            source.InputChanged -= SourceInputChanged;
        }

        /// <summary>
        /// Gets the current state of the inputTpye.
        /// <para>Implements <see cref="IDevice.Get(Enum)"/></para>
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

        private void SourceInputChanged(object sender, DeviceInputChangedEventArgs e)
        {
            RefreshInput();
        }

        /// <summary>
        /// Refreshes the current state. Triggers <see cref="InputChanged"/> event.
        /// </summary>
        /// <returns>if the input was available</returns>
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
            if (mapper.SelectedDPad != -1)
            {
                dPads[0] = source.DPads.ElementAt(mapper.SelectedDPad);
            }
            else
            {
                dPads[0] = DPadHelper.GetDirection(GetBool(XInputTypes.UP), GetBool(XInputTypes.DOWN), GetBool(XInputTypes.LEFT), GetBool(XInputTypes.RIGHT));
            }
            var changedDPads = state.SetDPads(dPads);
            var changedValues = state.SetValues(values.Where(t => !t.Key.IsDPad()).ToDictionary(x => (Enum)x.Key, x => x.Value));
            if (changedDPads.Any() || changedValues.Any())
                InputChanged?.Invoke(this, new DeviceInputChangedEventArgs(changedValues, changedDPads));
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

        /// <summary>
        /// Gets boolean output.
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>boolean value</returns>
        public bool GetBool(XInputTypes inputType)
        {
            return Get(inputType) > 0.5;
        }

        /// <summary>
        /// Gets the current state of the inputTpye.
        /// <para>Implements <see cref="IDevice.Get(Enum)"/></para>
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        public double Get(Enum inputType)
        {
            if (inputType is XInputTypes)
                return Get((XInputTypes)inputType);
            throw new ArgumentException();
        }
    }
}
