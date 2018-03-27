using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XOutput.Input.Keyboard
{
    /// <summary>
    /// Keyboard input device.
    /// </summary>
    public sealed class Keyboard : IInputDevice
    {
        public event Action InputChanged;
        public event Action Disconnected;

        public int ButtonCount => Enum.GetValues(typeof(Key)).Length;
        public string DisplayName => LanguageModel.Instance.Translate("Keyboard");
        public bool Connected => true;
        public bool HasDPad => false;

        public IEnumerable<DPadDirection> DPads => new DPadDirection[0];
        public IEnumerable<Enum> Buttons => buttons;
        public IEnumerable<Enum> Axes => new Enum[0];
        public IEnumerable<Enum> Sliders => new Enum[0];
        public int ForceFeedbackCount => 0;

        private Thread inputRefresher;
        private readonly Enum[] buttons;

        public Keyboard()
        {
            buttons = KeyboardInputHelper.Instance.Buttons.Where(x => x != Key.None).OrderBy(x => x.ToString()).OfType<Enum>().ToArray();
            inputRefresher = new Thread(() => InputRefresher());
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

        public double Get(Enum inputType)
        {
            if (!(inputType is Key))
                throw new ArgumentException();
            return System.Windows.Input.Keyboard.IsKeyDown((Key)inputType) ? 1 : 0;
        }

        /// <summary>
        /// Display name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DisplayName;
        }

        public void SetForceFeedback(short big, short small)
        {

        }

        private void InputRefresher()
        {
            try
            {
                while (true)
                {
                    InputChanged?.Invoke();
                    Thread.Sleep(1);
                }
            }
            catch (ThreadAbortException) { }
        }
    }
}
