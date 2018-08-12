using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

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
        public event DeviceDisconnectedHandler Disconnected { add { } remove { } }
        #endregion

        #region Properties
        public int ButtonCount => Enum.GetValues(typeof(Key)).Length;
        /// <summary>
        /// Gets the translated name of the keyboard.
        /// <para>Implements <see cref="IInputDevice.DisplayName"/></para>
        /// </summary>
        public string DisplayName => LanguageModel.Instance.Translate("Keyboard");
        /// <summary>
        /// Gets the keyboard string.
        /// <para>Implements <see cref="IInputDevice.Id"/></para>
        /// </summary>
        public string Id => "Keyboard";
        /// <summary>
        /// Returns true always, as keyboard is expected to be connected at all times.
        /// <para>Implements <see cref="IInputDevice.Connected"/></para>
        /// </summary>
        public bool Connected => true;

        /// <summary>
        /// Keyboards have no DPads.
        /// <para>Implements <see cref="IDevice.DPads"/></para>
        /// </summary>
        public IEnumerable<DPadDirection> DPads => new DPadDirection[0];
        /// <summary>
        /// Returns all know keys to keyboard.
        /// <para>Implements <see cref="IDevice.Buttons"/></para>
        /// </summary>
        public IEnumerable<InputType> Buttons => buttons;
        /// <summary>
        /// Keyboards have no axes.
        /// <para>Implements <see cref="IDevice.Axes"/></para>
        /// </summary>
        public IEnumerable<InputType> Axes => new InputType[0];
        /// <summary>
        /// Keyboards have no sliders.
        /// <para>Implements <see cref="IDevice.Sliders"/></para>
        /// </summary>
        public IEnumerable<InputType> Sliders => new InputType[0];
        /// <summary>
        /// Returns all know keys to keyboard.
        /// <para>Implements <see cref="IDevice.Values"/></para>
        /// </summary>
        public IEnumerable<InputType> Values => buttons;
        /// <summary>
        /// Keyboards have no force feedback motors.
        /// <para>Implements <see cref="IInputDevice.ForceFeedbackCount"/></para>
        /// </summary>
        public int ForceFeedbackCount => 0;
        #endregion

        private static readonly Keyboard instance = new Keyboard();
        public static Keyboard Instance => instance;
        private Thread inputRefresher;
        private readonly InputType[] buttons;
        private readonly DeviceState state;

        /// <summary>
        /// Creates a new keyboard device instance.
        /// </summary>
        private Keyboard()
        {
            buttons = ((Key[])Enum.GetValues(typeof(Key)))
                .Where(x => x != Key.None)
                .OrderBy(x => x.ToString())
                .Select(b => KeyboardInputHelper.Instance.ToInt(b + 1))
                .Distinct()
                .Select(b => new InputType
                {
                    Type = InputTypes.Button,
                    Count = b,
                }).ToArray();
            state = new DeviceState(buttons, 0);
            inputRefresher = new Thread(InputRefresher);
            inputRefresher.Name = "Keyboard input notification";
            inputRefresher.SetApartmentState(ApartmentState.STA);
            inputRefresher.IsBackground = true;
            inputRefresher.Start();
        }

        ~Keyboard()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes all resources.
        /// </summary>
        public void Dispose()
        {
            inputRefresher.Abort();
        }

        /// <summary>
        /// Gets the current state of the inputTpye.
        /// <para>Implements <see cref="IDevice.Get(Enum)"/></para>
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        public double Get(InputType inputType)
        {
            return System.Windows.Input.Keyboard.IsKeyDown(KeyboardInputHelper.Instance.ToKey(inputType.Count)) ? 1 : 0;
        }

        /// <summary>
        /// Gets the current raw state of the inputTpye.
        /// <para>Implements <see cref="IInputDevice.GetRaw(Enum)"/></para>
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        public double GetRaw(InputType inputType)
        {
            return Get(inputType);
        }

        /// <summary>
        /// Display name.
        /// <para>Overrides <see cref="object.ToString()"/></para>
        /// </summary>
        /// <returns>Friendly name</returns>
        public override string ToString()
        {
            return DisplayName;
        }

        /// <summary>
        /// This function does nothing. Keyboards have no force feedback motors.
        /// <para>Implements <see cref="IInputDevice.SetForceFeedback(double, double)"/></para>
        /// </summary>
        /// <param name="big">Big motor value</param>
        /// <param name="small">Small motor value</param>
        public void SetForceFeedback(double big, double small)
        {

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
                    var newValues = buttons.ToDictionary(t => t, t => Get(t));
                    var changedValues = state.SetValues(newValues);
                    if (changedValues.Any())
                        InputChanged?.Invoke(this, new DeviceInputChangedEventArgs(changedValues, new int[0]));
                    Thread.Sleep(ReadDelayMs);
                }
            }
            catch (ThreadAbortException) { }
        }

        public string ConvertToString(InputType type)
        {
            return KeyboardInputHelper.Instance.ConvertToString(type);
        }
    }
}
