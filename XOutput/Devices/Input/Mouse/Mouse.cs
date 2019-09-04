using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace XOutput.Devices.Input.Mouse
{
    /// <summary>
    /// Keyboard input device.
    /// </summary>
    public sealed class Mouse : IInputDevice
    {
        #region Constants
        /// <summary>
        /// The delay in milliseconds to sleep between input reads.
        /// </summary>
        public const int ReadDelayMs = 1;
        #endregion

        #region Events
        /// <summary>
        /// Triggered periodically to trigger input read from keyboards.
        /// <para>Implements <see cref="IDevice.InputChanged"/></para>
        /// </summary>
        public event DeviceInputChangedHandler InputChanged;
        /// <summary>
        /// Never used.
        /// <para>Implements <see cref="IInputDevice.Disconnected"/></para>
        /// </summary>
        public event DeviceDisconnectedHandler Disconnected;
        #endregion

        #region Properties
        public int ButtonCount => Enum.GetValues(typeof(MouseButton)).Length;
        /// <summary>
        /// Gets the translated name of the keyboard.
        /// <para>Implements <see cref="IInputDevice.DisplayName"/></para>
        /// </summary>
        public string DisplayName => LanguageModel.Instance.Translate("Mouse");
        /// <summary>
        /// Returns true always, as keyboard is expected to be connected at all times.
        /// <para>Implements <see cref="IInputDevice.Connected"/></para>
        /// </summary>
        public bool Connected => true;
        /// <summary>
        /// <para>Implements <see cref="IInputDevice.UniqueId"/></para>
        /// </summary>
        public string UniqueId => "Mouse";
        /// <summary>
        /// Keyboards have no DPads.
        /// <para>Implements <see cref="IDevice.DPads"/></para>
        /// </summary>
        public IEnumerable<DPadDirection> DPads => new DPadDirection[0];
        /// <summary>
        /// Returns all know keys to keyboard.
        /// <para>Implements <see cref="IDevice.Buttons"/></para>
        /// </summary>
        public IEnumerable<InputSource> Sources => sources;
        /// <summary>
        /// Keyboards have no force feedback motors.
        /// <para>Implements <see cref="IInputDevice.ForceFeedbackCount"/></para>
        /// </summary>
        public int ForceFeedbackCount => 0;
        /// <summary>
        /// <para>Implements <see cref="IInputDevice.InputConfiguration"/></para>
        /// </summary>
        public InputConfig InputConfiguration => inputConfig;
        public string HardwareID => null;
        #endregion

        private readonly Thread inputRefresher;
        private readonly MouseSource[] sources;
        private readonly DeviceState state;
        private readonly InputConfig inputConfig;
        private DeviceInputChangedEventArgs deviceInputChangedEventArgs;

        /// <summary>
        /// Creates a new keyboard device instance.
        /// </summary>
        public Mouse()
        {
            sources = Enum.GetValues(typeof(MouseButton)).OfType<MouseButton>().Select(x => new MouseSource(this, x.ToString(), x)).ToArray();
            state = new DeviceState(sources, 0);
            deviceInputChangedEventArgs = new DeviceInputChangedEventArgs(this);
            inputConfig = new InputConfig();
            inputRefresher = new Thread(InputRefresher);
            inputRefresher.Name = "Mouse input notification";
            inputRefresher.SetApartmentState(ApartmentState.STA);
            inputRefresher.IsBackground = true;
            inputRefresher.Start();
        }

        ~Mouse()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes all resources.
        /// </summary>
        public void Dispose()
        {
            Disconnected?.Invoke(this, new DeviceDisconnectedEventArgs());
            inputRefresher.Interrupt();
        }

        /// <summary>
        /// Gets the current state of the inputTpye.
        /// <para>Implements <see cref="IDevice.Get(InputSource)"/></para>
        /// </summary>
        /// <param name="inputType">Source of input</param>
        /// <returns>Value</returns>
        public double Get(InputSource source)
        {
            return source.Value;
        }

        /// <summary>
        /// Display name.
        /// <para>Overrides <see cref="object.ToString()"/></para>
        /// </summary>
        /// <returns>Friendly name</returns>
        public override string ToString()
        {
            return "Mouse";
        }

        /// <summary>
        /// This function does nothing. Keyboards have no force feedback motors.
        /// <para>Implements <see cref="IInputDevice.SetForceFeedback(double, double)"/></para>
        /// </summary>
        /// <param name="big">Big motor value</param>
        /// <param name="small">Small motor value</param>
        public void SetForceFeedback(double big, double small)
        {
            // Keyboard has no force feedback
        }

        /// <summary>
        /// Refreshes the current state. Triggers <see cref="InputChanged"/> event.
        /// </summary>
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
            catch (ThreadInterruptedException)
            {
                // Thread has been interrupted
            }
        }

        /// <summary>
        /// Refreshes the current state. Triggers <see cref="InputChanged"/> event.
        /// </summary>
        /// <returns>if the input was available</returns>
        public bool RefreshInput(bool force = false)
        {
            state.ResetChanges();
            App.Current.Dispatcher.Invoke(() =>
            {
                foreach (var source in sources)
                {
                    if (source.Refresh())
                    {
                        state.MarkChanged(source);
                    }
                }
            });
            var changes = state.GetChanges(force);
            if (changes.Any())
            {
                deviceInputChangedEventArgs.Refresh(changes);
                InputChanged?.Invoke(this, deviceInputChangedEventArgs);
            }
            return true;
        }
    }
}
