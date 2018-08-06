using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Devices.Input;
using XOutput.Devices.XInput.Settings;

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
        /// <summary>
        /// The delay in milliseconds to sleep between input reads.
        /// </summary>
        public const int ReadDelayMs = 1;
        #endregion

        #region Events
        /// <summary>
        /// This event is invoked if the data from the device was updated
        /// <para>Implements <see cref="IDevice.InputChanged"/></para>
        /// </summary>
        public event DeviceInputChangedHandler InputChanged;
        /// <summary>
        /// Triggered when the any read or write fails.
        /// <para>Implements <see cref="IInputDevice.Disconnected"/></para>
        /// </summary>
        public event DeviceDisconnectedHandler Disconnected;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the product name of the device.
        /// <para>Implements <see cref="IDevice.DisplayName"/></para>
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// <para>Implements <see cref="IDevice.DPads"/></para>
        /// </summary>
        public IEnumerable<DPadDirection> DPads => dPads;
        /// <summary>
        /// <para>Implements <see cref="IDevice.Buttons"/></para>
        /// </summary>
        public IEnumerable<InputType> Buttons => XInputHelper.Instance.Buttons.OfType<InputType>();
        /// <summary>
        /// <para>Implements <see cref="IDevice.Axes"/></para>
        /// </summary>
        public IEnumerable<InputType> Axes => XInputHelper.Instance.Axes.OfType<InputType>();
        /// <summary>
        /// XInput devices have no sliders.
        /// <para>Implements <see cref="IDevice.Sliders"/></para>
        /// </summary>
        public IEnumerable<InputType> Sliders => new InputType[0];
        /// <summary>
        /// <para>Implements <see cref="IDevice.Values"/></para>
        /// </summary>
        public IEnumerable<InputType> Values => alltypes;
        #endregion

        private readonly Dictionary<InputType, double> values = new Dictionary<InputType, double>();
        private readonly Dictionary<InputType, MapperSettings> mappers;
        private readonly DPadSettings dPadData;
        private readonly DPadDirection[] dPads = new DPadDirection[DPadCount];
        private readonly DeviceState state;
        private readonly InputType[] alltypes;
        private Thread inputRefresher;

        /// <summary>
        /// Creates a new XDevice.
        /// </summary>
        /// <param name="source">Direct input device</param>
        /// <param name="mapper">DirectInput to XInput mapper</param>
        public XOutputDevice(Dictionary<InputType, MapperSettings> mappers, DPadSettings dPadData)
        {
            alltypes = XInputHelper.Instance.Values.OfType<InputType>().ToArray();
            state = new DeviceState(alltypes.ToArray(), DPadCount);
            this.mappers = mappers;
            this.dPadData = dPadData;
            inputRefresher = new Thread(InputRefresher);
            inputRefresher.Name = ToString() + " input reader";
            inputRefresher.SetApartmentState(ApartmentState.STA);
            inputRefresher.IsBackground = true;
            inputRefresher.Start();
        }

        ~XOutputDevice()
        {
            Dispose();
        }

        public void Dispose()
        {
            inputRefresher?.Abort();
        }

        /// <summary>
        /// Gets the current state of the inputTpye.
        /// <para>Implements <see cref="IDevice.Get(Enum)"/></para>
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        public double Get(InputType inputType)
        {
            if (values.ContainsKey(inputType))
            {
                return values[inputType];
            }
            return 0;
        }

        private void InputRefresher()
        {
            try
            {
                while (true)
                {
                    RefreshInput();
                    Thread.Sleep(ReadDelayMs);
                }
            }
            catch (ThreadAbortException) { }
        }

        /// <summary>
        /// Refreshes the current state. Triggers <see cref="InputChanged"/> event.
        /// </summary>
        /// <returns>if the input was available</returns>
        public bool RefreshInput()
        {
            foreach (var type in alltypes)
            {
                var mapping = GetMapping(type);
                if (mapping != null)
                {
                    double value = 0;
                    if (mapping.Device != null)
                        value = mapping.Device.Get(mapping.InputType);
                    values[type] = mapping.GetValue(value);
                }
            }
            if (dPadData.HasDPad)
            {
                dPads[0] = dPadData.GetDirection();
            }
            else
            {
                //dPads[0] = DPadHelper.GetDirection(GetBool(XInputTypes.UP), GetBool(XInputTypes.DOWN), GetBool(XInputTypes.LEFT), GetBool(XInputTypes.RIGHT));
            }
            var changedDPads = state.SetDPads(dPads);
            var changedValues = state.SetValues(values.Where(t => !t.Key.IsOther()).ToDictionary(x => x.Key, x => x.Value));
            if (changedDPads.Any() || changedValues.Any())
                InputChanged?.Invoke(this, new DeviceInputChangedEventArgs(changedValues, changedDPads));
            return true;
        }

        /// <summary>
        /// Gets a snapshot of data.
        /// </summary>
        /// <returns></returns>
        public Dictionary<InputType, double> GetValues()
        {
            var newValues = new Dictionary<InputType, double>(values);
            /*newValues[XInputTypes.UP] = dPads[0].HasFlag(DPadDirection.Up) ? 1 : 0;
            newValues[XInputTypes.LEFT] = dPads[0].HasFlag(DPadDirection.Left) ? 1 : 0;
            newValues[XInputTypes.RIGHT] = dPads[0].HasFlag(DPadDirection.Right) ? 1 : 0;
            newValues[XInputTypes.DOWN] = dPads[0].HasFlag(DPadDirection.Down) ? 1 : 0;*/
            return newValues;
        }

        internal DPadSettings GetDPadData()
        {
            return dPadData;
        }

        /// <summary>
        /// Gets boolean output.
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>boolean value</returns>
        public bool GetBool(InputType inputType)
        {
            return Get(inputType) > 0.5;
        }

        public MapperSettings GetMapping(InputType type)
        {
            if (!mappers.ContainsKey(type))
            {
                mappers[type] = new MapperSettings();
            }
            return mappers[type];
        }

        public string ConvertToString(InputType type)
        {
            if (type.IsAxis())
            {
                return "A" + type.Count;
            }
            else if (type.IsButton())
            {
                return "B" + type.Count;
            }
            else if (type.IsSlider())
            {
                return "S" + type.Count;
            }
            return "DISABLED";
        }
    }
}
