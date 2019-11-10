using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XOutput.Devices.Input.Mouse
{
    public class MouseHook : IDisposable
    {
        public event MouseHookEvent MouseEvent;
        private IntPtr hookPtr = IntPtr.Zero;
        private HookProc hook;

        public void StartHook()
        {
            hook = (nCode, wParam, lParam) =>
            {
                if (nCode >= 0)
                {
                    var args = MouseHookEventArgs.Create((MouseMessage)wParam, lParam);
                    if (args != null)
                    {
                        MouseEvent?.Invoke(args);
                    }
                }
                return NativeMethods.CallNextHookEx(hookPtr, nCode, wParam, lParam);
            };
            hookPtr = NativeMethods.SetWindowsHookEx(HookType.WH_MOUSE_LL, hook, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
            if (hookPtr == IntPtr.Zero)
            {
                throw new Win32Exception("Unable to set MouseHook");
            }
        }

        public void Dispose()
        {
            if (hookPtr != IntPtr.Zero)
            {
                if (!NativeMethods.UnhookWindowsHookEx(hookPtr))
                {
                    throw new Win32Exception("Unable to clear MouseHook");
                }
                hookPtr = IntPtr.Zero;
            }
        }
    }

    public class MouseHookEventArgs
    {
        public MouseButton Button { get; set; }
        public MouseButtonState State { get; set; }

        internal MouseHookEventArgs(MouseButton button, MouseButtonState state)
        {
            Button = button;
            State = state;
        }

        internal static MouseHookEventArgs Create(MouseMessage wParam, IntPtr lParam)
        {
            switch(wParam)
            {
                case MouseMessage.WM_LBUTTONDOWN:
                    return new MouseHookEventArgs(MouseButton.Left, MouseButtonState.Pressed);
                case MouseMessage.WM_LBUTTONUP:
                    return new MouseHookEventArgs(MouseButton.Left, MouseButtonState.Released);
                case MouseMessage.WM_RBUTTONDOWN:
                    return new MouseHookEventArgs(MouseButton.Right, MouseButtonState.Pressed);
                case MouseMessage.WM_RBUTTONUP:
                    return new MouseHookEventArgs(MouseButton.Right, MouseButtonState.Released);
                case MouseMessage.WM_MBUTTONDOWN:
                    return new MouseHookEventArgs(MouseButton.Middle, MouseButtonState.Pressed);
                case MouseMessage.WM_MBUTTONUP:
                    return new MouseHookEventArgs(MouseButton.Middle, MouseButtonState.Released);
                case MouseMessage.WM_XBUTTONDOWN:
                    return new MouseHookEventArgs(GetButton(lParam), MouseButtonState.Pressed);
                case MouseMessage.WM_XBUTTONUP:
                    return new MouseHookEventArgs(GetButton(lParam), MouseButtonState.Released);
                default:
                    return null;
            }
        }

        internal static MouseButton GetButton(IntPtr lParam)
        {
            MSLLHOOKSTRUCT msLLHookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            return (msLLHookStruct.mouseData >> 16) == 1 ? MouseButton.XButton1 : MouseButton.XButton2;
        }
    }

    public delegate void MouseHookEvent(MouseHookEventArgs args);

    internal delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    internal class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc callback, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    }

    internal enum HookType
    {
        WH_KEYBOARD = 2,
        WH_MOUSE = 7,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE_LL = 14
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEHOOKSTRUCT
    {
        public POINT pt;
        public int hwnd;
        public int wHitTestCode;
        public int dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public int mouseData;
        public int flags;
        public int time;
        public IntPtr dwExtraInfo;
    }

    internal enum MouseMessage
    {
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,
        WM_XBUTTONDOWN = 0x020B,
        WM_XBUTTONUP = 0x020C,

        WM_MOUSEWHEEL = 0x020A,
        WM_MOUSEHWHEEL = 0x020E,

        WM_NCMOUSEMOVE = 0x00A0,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONUP = 0x00A2,
        WM_NCLBUTTONDBLCLK = 0x00A3,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_NCRBUTTONUP = 0x00A5,
        WM_NCRBUTTONDBLCLK = 0x00A6,
        WM_NCMBUTTONDOWN = 0x00A7,
        WM_NCMBUTTONUP = 0x00A8,
        WM_NCMBUTTONDBLCLK = 0x00A9
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }

    internal enum KeyboardMessage
    {
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x0101,
        WM_SYSKEYDOWN = 0x0104,
        WM_SYSKEYUP = 0x0105
    }
}
