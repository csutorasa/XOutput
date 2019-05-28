using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XOutput.Devices.Input.Keyboard
{
    /// <summary>
    /// <see cref="IInputHelper{T}"/> for <see cref="Key"/>.
    /// </summary>
    public class KeyboardInputHelper : AbstractInputHelper<Key>
    {
        protected static readonly KeyboardInputHelper instance = new KeyboardInputHelper();
        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        public static KeyboardInputHelper Instance => instance;

        /// <summary>
        /// Returns false. Keyboards have no axes.
        /// <para>Implements <see cref="IInputHelper{T}.IsAxis(T)"/> enum value</para>
        /// </summary>
        /// <param name="type"><see cref="DirectInputTypes"/> enum value</param>
        /// <returns></returns>
        public override bool IsAxis(Key type)
        {
            return false;
        }

        /// <summary>
        /// Return true. Keyboards have only buttons.
        /// <para>Implements <see cref="IInputHelper{T}.IsButton(T)"/> enum value</para>
        /// </summary>
        /// <param name="type"><see cref="DirectInputTypes"/> enum value</param>
        /// <returns></returns>
        public override bool IsButton(Key type)
        {
            return true;
        }

        /// <summary>
        /// Returns false. Keyboards have no DPads.
        /// <para>Implements <see cref="IInputHelper{T}.IsDPad(T)"/> enum value</para>
        /// </summary>
        /// <param name="type"><see cref="DirectInputTypes"/> enum value</param>
        /// <returns></returns>
        public override bool IsDPad(Key type)
        {
            return false;
        }

        /// <summary>
        /// Returns false. Keyboards have no sliders.
        /// <para>Implements <see cref="IInputHelper{T}.IsSlider(T)"/> enum value</para>
        /// </summary>
        /// <param name="type"><see cref="DirectInputTypes"/> enum value</param>
        /// <returns></returns>
        public override bool IsSlider(Key type)
        {
            return false;
        }
    }
}
