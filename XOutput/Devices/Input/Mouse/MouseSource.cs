using System.Windows.Input;
using XOutput.Tools;

namespace XOutput.Devices.Input.Mouse
{
    /// <summary>
    /// Direct input source.
    /// </summary>
    public class MouseSource : InputSource
    {
        private readonly MouseButton key;
        private double state = 0;

        public MouseSource(IInputDevice inputDevice, string name, MouseButton key) : base(inputDevice, name, InputSourceTypes.Button, (int)key)
        {
            this.key = key;
            ApplicationContext.Global.Resolve<MouseHook>().MouseEvent += MouseEventHandler;
        }

        private void MouseEventHandler(MouseHookEventArgs args)
        {
            if (args.Button == key)
            {
                state = args.State == MouseButtonState.Pressed ? 1 : 0;
            }
        }

        internal bool Refresh()
        {
            return RefreshValue(state);
        }
    }
}
