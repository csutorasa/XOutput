using System;

namespace XOutput.Core.Threading
{
    public static class WindowHandleStore
    {
        public static event WindowEvent WindowEvent;
        public static IntPtr Handle { get; set; }

        public static void HandleEvent(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var args = new WindowEventArgs(hwnd, msg, wParam, lParam, handled);
            WindowEvent?.Invoke(Handle, args);
            handled = args.Handled;
        }
    }

    public delegate void WindowEvent(object sender, WindowEventArgs args);

    public class WindowEventArgs {
        public IntPtr Hwnd { get; private set; }
        public int Msg { get; private set; }
        public IntPtr WParam { get; private set; }
        public IntPtr LParam { get; private set; }
        public bool Handled { get; set; }

        internal WindowEventArgs(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, bool handled)
        {
            Hwnd = hwnd;
            Msg = msg;
            WParam = wParam;
            LParam = lParam;
            Handled = handled;
        }
    }
}
