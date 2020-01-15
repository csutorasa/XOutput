using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using XOutput.Core.Threading;

namespace XOutput.Devices.Input.Keyboard
{
    /// <summary>
    /// Keyboard input device.
    /// </summary>
    public sealed class Keyboard : IInputDevice
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
        public int ButtonCount => Enum.GetValues(typeof(Key)).Length;
        /// <summary>
        /// Gets the translated name of the keyboard.
        /// <para>Implements <see cref="IInputDevice.DisplayName"/></para>
        /// </summary>
        public string DisplayName => LanguageModel.Instance.Translate("Keyboard");
        /// <summary>
        /// Returns true always, as keyboard is expected to be connected at all times.
        /// <para>Implements <see cref="IInputDevice.Connected"/></para>
        /// </summary>
        public bool Connected => true;
        /// <summary>
        /// <para>Implements <see cref="IInputDevice.UniqueId"/></para>
        /// </summary>
        public string UniqueId => "Keyboard";
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

        private readonly ThreadContext inputRefresher;
        private readonly KeyboardSource[] sources;
        private readonly DeviceState state;
        private readonly InputConfig inputConfig;
        private DeviceInputChangedEventArgs deviceInputChangedEventArgs;
        private bool disposed = false;

        /// <summary>
        /// Creates a new keyboard device instance.
        /// </summary>
        public Keyboard()
        {
            sources = Enum.GetValues(typeof(Key)).OfType<Key>().Where(x => x != Key.None).OrderBy(x => x.ToString()).Select(x => new KeyboardSource(this, x.ToString(), x)).ToArray();
            state = new DeviceState(sources, 0);
            deviceInputChangedEventArgs = new DeviceInputChangedEventArgs(this);
            inputConfig = new InputConfig();
            inputRefresher = ThreadCreator.Create("Keyboard input notification", InputRefresher).SetApartmentState(ApartmentState.STA).Start();
        }

        /// <summary>
        /// Disposes all resources.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                inputRefresher?.Cancel().Wait();
            }
        }

        /// <summary>
        /// Display name.
        /// <para>Overrides <see cref="object.ToString()"/></para>
        /// </summary>
        /// <returns>Friendly name</returns>
        public override string ToString()
        {
            return "Keyboard";
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
        private void InputRefresher(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                RefreshInput();
                Thread.Sleep(ReadDelayMs);
            }
        }

        /// <summary>
        /// Refreshes the current state. Triggers <see cref="InputChanged"/> event.
        /// </summary>
        /// <returns>if the input was available</returns>
        public bool RefreshInput(bool force = false)
        {
            state.ResetChanges();
            foreach (var source in sources)
            {
                if (source.Refresh())
                {
                    state.MarkChanged(source);
                }
            }
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
