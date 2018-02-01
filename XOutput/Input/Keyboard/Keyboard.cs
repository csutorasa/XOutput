using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XOutput.Input.Keyboard
{
    public class Keyboard : IInputDevice
    {
        public int ButtonCount => Enum.GetValues(typeof(Key)).Length;
        public string DisplayName => "Keyboard";
        public bool Connected => true;
        public bool HasAxes => false;
        public bool HasDPad => false;
        public DPadDirection DPad => DPadDirection.None;

        public event Action InputChanged;
        private Thread inputRefresher;

        public Keyboard()
        {
            inputRefresher = new Thread(() => {
                try
                {
                    while (true)
                    {
                        InputChanged?.Invoke();
                        Thread.Sleep(1);
                    }
                }
                catch (ThreadAbortException) { }
            });
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

        public IEnumerable<Enum> GetButtons()
        {
            return ((Key[])Enum.GetValues(typeof(Key)))
                .Where(x => x != Key.None)
                .OrderBy(x => x.ToString())
                .Select(x => (Enum)x);
        }

        public IEnumerable<Enum> GetAxes()
        {
            return new Enum[0];
        }

        /// <summary>
        /// Display name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DisplayName;
        }
    }
}
