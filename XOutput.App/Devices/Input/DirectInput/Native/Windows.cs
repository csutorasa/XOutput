using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace XOutput.App.Devices.Input.DirectInput.Native
{
    public static class WindowHandle {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public static IntPtr GetCurrentModuleHandle() {
            return GetModuleHandle(null);
        }
    }

    public abstract class DllLibrary : IDisposable {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string libPath);
        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        private IntPtr hModule;
        private bool disposed = false;

        protected DllLibrary(string libPath)
        {
            hModule = LoadLibrary(libPath);
            if (hModule == IntPtr.Zero)
            {
                var exception = Marshal.GetExceptionForHR(Marshal.GetLastWin32Error());
                if (exception != null)
                {
                    throw exception;
                }
            }
        }

        protected T GetProcedure<T>(string procedureName) where T : Delegate
        {
            IntPtr procAddress = GetProcAddress(hModule, procedureName);
            if (hModule == IntPtr.Zero)
            {
                var exception = Marshal.GetExceptionForHR(Marshal.GetLastWin32Error());
                if (exception != null)
                {
                    throw exception;
                }
            }
            return (T) Marshal.GetDelegateForFunctionPointer(procAddress, typeof(T));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing && hModule != IntPtr.Zero)
            {
                if(!FreeLibrary(hModule))
                {
                    var exception = Marshal.GetExceptionForHR(Marshal.GetLastWin32Error());
                    if (exception != null)
                    {
                        throw exception;
                    }
                }
            }
            disposed = true;
        }
    }
}
