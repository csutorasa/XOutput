using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Devices.Input;

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
        private readonly Dictionary<XInputTypes, MapperData> mappers;
        private readonly DPadData dPadData;
        private readonly DPadDirection[] dPads = new DPadDirection[DPadCount];
        private readonly DeviceState state;
        private Thread inputRefresher;

        /// <summary>
        /// Creates a new XDevice.
        /// </summary>
        /// <param name="source">Direct input device</param>
        /// <param name="mapper">DirectInput to XInput mapper</param>
        public XOutputDevice(Dictionary<XInputTypes, MapperData> mappers, DPadData dPadData)
        {
            state = new DeviceState(XInputHelper.Instance.Values.OfType<Enum>().ToArray(), DPadCount);
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
        public double Get(XInputTypes inputType)
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
            foreach (var type in XInputHelper.Instance.Values)
            {
                var mapping = GetMapping(type);
                if (mapping != null)
                {
                    double value = 0;
                    if (mapping.InputDevice != null && mapping.InputType != null)
                        value = mapping.InputDevice.Get(mapping.InputType);
                    values[type] = mapping.GetValue(value);
                }
            }
            if (dPadData.HasDPad)
            {
                dPads[0] = dPadData.GetDirection();
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

        internal DPadData GetDPadData()
        {
            return dPadData;
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

        public MapperData GetMapping(XInputTypes type)
        {
            return mappers[type];
        }
    }
}
