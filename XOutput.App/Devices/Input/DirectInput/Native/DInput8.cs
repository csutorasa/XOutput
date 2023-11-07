using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace XOutput.App.Devices.Input.DirectInput.Native
{
    public class DInput8 : DllLibrary
    {
        private delegate HResult DirectInput8CreateDelegate(IntPtr hinst, uint dwVersion, Guid riidltf, out IntPtr ppvOut, IntPtr punkOuter);

        public DInput8() : base("dinput8.dll")
        {

        }

        public IDirectInput8 DirectInput8Create(IntPtr hinst) {
            var directInput8Create = GetProcedure<DirectInput8CreateDelegate>("DirectInput8Create");
            IntPtr value;
            directInput8Create(hinst, 0x00000800, IID.IID_IDirectInput8W, out value, IntPtr.Zero);
            //return (IDirectInput8)Marshal.GetObjectForIUnknown(value);
            return null;
        }
    }

    public interface IDirectInput8
    {
        HResult ConfigureDevices(DIConfigureDevicesCallback lpdiCallback, DiConfigureDevicesParams lpdiCDParams, uint dwFlags, IntPtr pvRefData);
        HResult CreateDevice(Guid rguid, ref IntPtr lplpDirectInputDevice, IntPtr pUnkOuter);
        HResult EnumDevices(uint dwDevType, DIConfigureDevicesCallback lpCallback, IntPtr pvRef, uint dwFlags);
        HResult EnumDevicesBySemantics(string ptszUserName, DiActionFormat lpdiActionFormat, DiEnumDevicesBySemanticsCallback lpCallback, IntPtr pvRef, uint dwFlags);
        HResult FindDevice(Guid rguidClass, string ptszName, Guid pguidInstance);
        HResult GetDeviceStatus(Guid guid);
        HResult Initialize(IntPtr hinst, uint dwVersion);
        HResult RunControlPanel(IntPtr hwndOwner, uint dwFlags);
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
        public static readonly Guid IID_IDirectInputDeviceA = new Guid("5944e680-c92e-11cf-bfc7-444553540000");
        public static readonly Guid IID_IDirectInputDeviceW = new Guid("5944e681-c92e-11cf-bfc7-444553540000");
        public static readonly Guid IID_IDirectInputDevice2A = new Guid("5944e682-c92e-11cf-bfc7-444553540000");
        public static readonly Guid IID_IDirectInputDevice2W = new Guid("5944e683-c92e-11cf-bfc7-444553540000");
        public static readonly Guid IID_IDirectInputDevice7A = new Guid("57d7c6bc-2356-11d3-8e9d-00c04f6844ae");
        public static readonly Guid IID_IDirectInputDevice7W = new Guid("57d7c6bd-2356-11d3-8e9d-00c04f6844ae");
        public static readonly Guid IID_IDirectInputDevice8A = new Guid("54d41080-dc15-4833-a41b-748f73a38179");
        public static readonly Guid IID_IDirectInputDevice8W = new Guid("54d41081-dc15-4833-a41b-748f73a38179");
        public static readonly Guid IID_IDirectInputEffect = new Guid("e7e1f7c0-88d2-11d0-9ad0-00a0c9a06e35");
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DiEnumDevicesBySemanticsCallback
    {
        
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DiConfigureDevicesParams
    {

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DiActionFormat
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

    public struct DiDeviceInstance
    {

    }

    public delegate Bool DIEnumDevicesCallback(DiDeviceInstance lpddi, int pvRef);

    public delegate Bool DIEnumDevicesBySemanticsCallback(IntPtr lpddi, DiDeviceInstance lpdid, uint dwFlags, uint dwRemaining, IntPtr pvRef);

    public delegate Bool DIConfigureDevicesCallback(IntPtr lpDDSTarget, IntPtr pvRef);

    public enum HResult : uint
    {
        DI_OK = 0x00000000,
        DIERR_BETADIRECTINPUTVERSION,
        DIERR_OLDDIRECTINPUTVERSION,
        DIERR_INVALIDPARAM = 0x80070057,
        //DIERR_NOTINITIALIZED = 0,
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
