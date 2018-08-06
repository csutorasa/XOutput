using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XOutput.Devices.Input.Keyboard
{
    public class KeyboardInputHelper
    {
        protected static readonly KeyboardInputHelper instance = new KeyboardInputHelper();
        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        public static KeyboardInputHelper Instance => instance;

        public int ToInt(Key key)
        {
            return (int)key;
        }

        public Key ToKey(int key)
        {
            return (Key)key;
        }

        public string ConvertToString(InputType type)
        {
            return ((Key)type.Count).ToString();
        }
    }
}
