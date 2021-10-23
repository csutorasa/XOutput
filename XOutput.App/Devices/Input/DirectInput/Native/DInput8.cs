using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.App.Devices.Input.DirectInput.Native
{
    public static class DInput8
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("dinput8.dll", SetLastError = true)]
        public static extern HResult DirectInput8Create(IntPtr hinst, uint dwVersion, Guid riidltf, out IntPtr ppvOut, IntPtr punkOuter);
    }

    public interface IDirectInput8W
    {
        object InitializeLifetimeService();
        object GetLifetimeService();
    }

    public class ComObject
    {
        private readonly object nativeObject;
        private readonly Type type;

        public ComObject(IntPtr nativePointer)
        {
            nativeObject = Marshal.GetObjectForIUnknown(nativePointer);
            type = nativeObject.GetType();
        }

        protected object ProxyToNative(string methodName, params object[] parameters)
        {
            return ProxyToNative(methodName, parameters.Select(p => p.GetType()).ToArray(), parameters);
        }

        protected object ProxyToNative(MethodBase methodInfo, params object[] parameters)
        {
            return ProxyToNative(methodInfo.Name, methodInfo.GetParameters().Select(p => p.ParameterType).ToArray(), parameters);
        }

        protected object ProxyToNative(string methodName, Type[] types, object[] parameters)
        {
            return type.GetMethod(methodName, types).Invoke(nativeObject, parameters);
        }
    }

    public class DirectInput8W  : ComObject
    {
        public DirectInput8W(IntPtr nativePointer) : base(nativePointer)
        {

        }

        public object InitializeLifetimeService()
        {
            return ProxyToNative(MethodBase.GetCurrentMethod());
        }

        public object GetLifetimeService()
        {
            return ProxyToNative(MethodBase.GetCurrentMethod());
        }
    }

    public class Test
    {
        public static HResult EnumDevices(DwDeviceType dwDevice, LpDiEnumDevicesCallback lpCallback, int pvRef, EnumDevicesFlags dwFlags)
        {
            return HResult.DI_OK;
        }
    }

    public static class IID
    {
        public static readonly Guid IID_IDirectInputA = new Guid("89521360-aa8a-11cf-bfc7-444553540000");
        public static readonly Guid IID_IDirectInput2A = new Guid("5944e662-aa8a-11cf-bfc7-444553540000");
        public static readonly Guid IID_IDirectInput2W = new Guid("5944e663-aa8a-11cf-bfc7-444553540000");
        public static readonly Guid IID_IDirectInput7A = new Guid("9a4cb684-236d-11d3-8e9d-00c04f6844ae");
        public static readonly Guid IID_IDirectInput7W = new Guid("9a4cb685-236d-11d3-8e9d-00c04f6844ae");
        public static readonly Guid IID_IDirectInput8A = new Guid("bf798030-483a-4da2-aa99-5d64ed369700");
        public static readonly Guid IID_IDirectInput8W = new Guid("bf798031-483a-4da2-aa99-5d64ed369700");
        public static readonly Guid TEST = new Guid("54d41081-dc15-4833-a41b-748f73a38179");
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct LpDiConfigureDevicesCallback
    {

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct LpDiActionFormat
    {
        /*uint dwSize;
        uint dwActionSize;
        uint dwDataSize;
        uint dwNumActions;
        LPDIACTION rgoAction;
        GUID guidActionMap;
        uint dwGenre;
        uint dwBufferSize;
        int lAxisMin;
        int lAxisMax;
        HINSTANCE hInstString;
        FILETIME ftTimeStamp;
        uint dwCRC;
        TCHAR tszActionMap[MAX_PATH];*/
    }

    public struct LpDiDeviceInstance
    {

    }

    public delegate Bool LpDiEnumDevicesCallback(LpDiDeviceInstance lpddi, int pvRef);

    public enum HResult : uint
    {
        DI_OK = 0x00000000,
        DIERR_BETADIRECTINPUTVERSION,
        DIERR_OLDDIRECTINPUTVERSION,
        DIERR_INVALIDPARAM = 0x80070057,
        DIERR_NOTINITIALIZED = 0,
        DIERR_OUTOFMEMORY = 0x8007000E,
    }

    public enum EnumDevicesFlags : uint
    {
        DIEDFL_ALLDEVICES = 0x00000000,
        DIEDFL_ATTACHEDONLY = 0x00000001,
        DIEDFL_FORCEFEEDBACK = 0x00000100,
        DIEDFL_INCLUDEALIASES = 0x00010000,
        DIEDFL_INCLUDEPHANTOMS = 0x00020000,
        DIEDFL_INCLUDEHIDDEN = 0x00040000,
    }

    public enum DwDeviceType
    {
        DI8DEVCLASS_ALL,
        DI8DEVCLASS_DEVICE,
        DI8DEVCLASS_GAMECTRL,
        DI8DEVCLASS_KEYBOARD,
        DI8DEVCLASS_POINTER,
    }

    public enum Bool
    {
        DIENUM_STOP = 0,
        DIENUM_CONTINUE = 1,
    }
}
