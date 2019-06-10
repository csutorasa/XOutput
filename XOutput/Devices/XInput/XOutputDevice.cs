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
        public IEnumerable<InputSource> Sources => sources;
        #endregion

        private readonly IInputDevice source;
        private readonly InputMapperBase mapper;
        private readonly DPadDirection[] dPads = new DPadDirection[DPadCount];
        private readonly XOutputSource[] sources;
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
            sources = XInputHelper.Instance.GenerateSources();
            state = new DeviceState(sources, DPadCount);
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
        public double Get(InputSource inputType)
        {
            return inputType.Value;
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
            state.ResetChanges();
            foreach (var s in sources)
            {
                if (s.Refresh(source, mapper))
                {
                    state.MarkChanged(s);
                }
            }
            var changes = state.GetChanges();
            if (mapper.SelectedDPad != -1)
            {
                dPads[0] = source.DPads.ElementAt(mapper.SelectedDPad);
            }
            else
            {
                dPads[0] = DPadHelper.GetDirection(GetBool(XInputTypes.UP), GetBool(XInputTypes.DOWN), GetBool(XInputTypes.LEFT), GetBool(XInputTypes.RIGHT));
            }
            state.SetDPad(0, dPads[0]);
            var changedDPads = state.GetChangedDpads();
            if (changedDPads.Any() || changes.Any())
                InputChanged?.Invoke(this, new DeviceInputChangedEventArgs(changes, changedDPads));
            return true;
        }

        /// <summary>
        /// Gets a snapshot of data.
        /// </summary>
        /// <returns></returns>
        public Dictionary<XInputTypes, double> GetValues()
        {
            var newValues = new Dictionary<XInputTypes, double>();
            foreach (var source in sources)
            {
                newValues[source.XInputType] = source.Value;
            }
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
            // FIXME
            return GetValues()[(XInputTypes)inputType];
        }
    }
}
