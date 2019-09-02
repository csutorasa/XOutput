using System.Windows.Input;

namespace XOutput.Devices.Input.Mouse
{
    /// <summary>
    /// Direct input source.
    /// </summary>
    public class MouseSource : InputSource
    {
        private readonly MouseButton key;

        public MouseSource(IInputDevice inputDevice, string name, MouseButton key) : base(inputDevice, name, InputSourceTypes.Button, (int)key)
        {
            this.key = key;
        }

        internal bool Refresh()
        {
            double newValue = 0;
            App.Current.Dispatcher.Invoke(() =>
            {
                switch (key)
                {
                    case MouseButton.Left:
                        newValue = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed ? 1 : 0;
                        break;
                    case MouseButton.Right:
                        newValue = System.Windows.Input.Mouse.RightButton == MouseButtonState.Pressed ? 1 : 0;
                        break;
                    case MouseButton.Middle:
                        newValue = System.Windows.Input.Mouse.MiddleButton == MouseButtonState.Pressed ? 1 : 0;
                        break;
                    case MouseButton.XButton1:
                        newValue = System.Windows.Input.Mouse.XButton1 == MouseButtonState.Pressed ? 1 : 0;
                        break;
                    case MouseButton.XButton2:
                        newValue = System.Windows.Input.Mouse.XButton2 == MouseButtonState.Pressed ? 1 : 0;
                        break;
                    default:
                        break;
                }
            });
            return RefreshValue(newValue);
        }
    }
}
