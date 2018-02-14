using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XOutput.Input.Keyboard
{
    public class KeyboardInputHelper : AbstractInputHelper<Key>
    {
        public static readonly KeyboardInputHelper instance = new KeyboardInputHelper();
        public static KeyboardInputHelper Instance => instance;

        public override bool IsAxis(Key type)
        {
            return false;
        }

        public override bool IsButton(Key type)
        {
            return true;
        }

        public override bool IsDPad(Key type)
        {
            return false;
        }

        public override bool IsSlider(Key type)
        {
            return false;
        }
    }
}
