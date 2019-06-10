using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XOutput.Devices.Input.Keyboard
{
    /// <summary>
    /// Direct input source.
    /// </summary>
    public class KeyboardSource : InputSource
    {
        private Key key;

        public KeyboardSource(string name, Key key) : base(name, InputSourceTypes.Button)
        {
            this.key = key;
        }

        internal bool Refresh()
        {
            double newValue = System.Windows.Input.Keyboard.IsKeyDown(key) ? 1 : 0;
            return RefreshValue(newValue);
        }
    }
}
