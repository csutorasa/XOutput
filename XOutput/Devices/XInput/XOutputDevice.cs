﻿using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Devices.Input;
using XOutput.Devices.Mapper;
using XOutput.Logging;

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
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(XOutputDevice));
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

        private IEnumerable<IInputDevice> boundSources = new List<IInputDevice>();
        private readonly InputMapper mapper;
        private readonly DPadDirection[] dPads = new DPadDirection[DPadCount];
        private readonly XOutputSource[] sources;
        private DeviceInputChangedEventArgs deviceInputChangedEventArgs;

        /// <summary>
        /// Creates a new XDevice.
        /// </summary>
        /// <param name="source">Direct input device</param>
        /// <param name="mapper">DirectInput to XInput mapper</param>
        public XOutputDevice(InputMapper mapper)
        {
            this.mapper = mapper;
            sources = XInputHelper.Instance.GenerateSources();
            deviceInputChangedEventArgs = new DeviceInputChangedEventArgs(this);
        }

        ~XOutputDevice()
        {
            Dispose();
        }

        public void UpdateSources(IEnumerable<IInputDevice> sources)
        {
            foreach (var source in boundSources)
            {
                source.InputChanged -= SourceInputChanged;
            }
            boundSources = sources;
            foreach (var source in boundSources)
            {
                source.InputChanged += SourceInputChanged;
            }
            RefreshInput(true);
        }

        public void Dispose()
        {
            foreach (var source in boundSources)
            {
                source.InputChanged -= SourceInputChanged;
            }
        }

        /// <summary>
        /// Gets the current state of the inputTpye.
        /// <para>Implements <see cref="IDevice.Get(InputSource)"/></para>
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        public double Get(InputSource source)
        {
            return source.Value;
        }

        private void SourceInputChanged(object sender, DeviceInputChangedEventArgs e)
        {
            RefreshInput();
        }

        /// <summary>
        /// Refreshes the current state. Triggers <see cref="InputChanged"/> event.
        /// </summary>
        /// <returns>if the input was available</returns>
        public bool RefreshInput(bool force = false)
        {
            DeviceState state = new DeviceState(sources, DPadCount);
            foreach (var s in sources)
            {
                if (s.Refresh(mapper))
                {
                    state.MarkChanged(s);
                }
            }
            var changes = state.GetChanges(force);
            dPads[0] = DPadHelper.GetDirection(GetBool(XInputTypes.UP), GetBool(XInputTypes.DOWN), GetBool(XInputTypes.LEFT), GetBool(XInputTypes.RIGHT));
            state.SetDPad(0, dPads[0]);
            var changedDPads = state.GetChangedDpads(force);
            if (changedDPads.Any() || changes.Any())
            {
                deviceInputChangedEventArgs.Refresh(changes, changedDPads);
                InputChanged?.Invoke(this, deviceInputChangedEventArgs);
            }
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
            XInputTypes? type = inputType as XInputTypes?;
            if (type.HasValue)
            {
                return sources.First(s => s.XInputType == type.Value).Value;
            }
            return 0;
        }
    }
}
