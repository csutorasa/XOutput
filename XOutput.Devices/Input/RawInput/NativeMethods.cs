using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Threading;

namespace XOutput.Devices.Input.RawInput
{
    internal class Info
    {
        public string Name { get; private set; }
        public uint VendorId { get; private set; }
        public uint ProductId { get; private set; }
        public uint VersionNumber { get; private set; }
        public ushort Usage { get; private set; }
        public ushort UsagePage { get; private set; }

        public Info(string name, uint vendorID, uint productId, uint versionNumber, ushort usage, ushort usagePage)
        {
            Name = name;
            VendorId = vendorID;
            ProductId = productId;
            VersionNumber = versionNumber;
            Usage = usage;
            UsagePage = usagePage;
        }

        internal string ToHidString()
        {
            string vendor = VendorId.ToString("X4").ToUpper();
            string product = ProductId.ToString("X4").ToUpper();
            if (VersionNumber == 0)
            {
                return $"HID\\VID_{vendor}&PID_{product}";
            }
            else
            {
                string version = VersionNumber.ToString("X4").ToUpper();
                return $"HID\\VID_{vendor}&PID_{product}&REV_{version}";
            }
        }
    }

#pragma warning disable 0649
    internal enum RawInputDeviceType : uint
    {
        Mouse = 0,
        Keyboard = 1,
        HumanInterfaceDevice = 2
    }

    internal enum InfoCommand : uint
    {
        PreparsedData = 0x20000005,
        DeviceName = 0x20000007,
        DeviceInfo = 0x2000000b,
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DeviceInfo
    {
        [FieldOffset(0)]
        public int Size;
        [FieldOffset(4)]
        public int Type;
        [FieldOffset(8)]
        public DeviceInfoMouse MouseInfo;
        [FieldOffset(8)]
        public DeviceInfoKeyboard KeyboardInfo;
        [FieldOffset(8)]
        public DeviceInfoHID HIDInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DeviceInfoMouse
    {
        public uint ID;
        public uint NumberOfButtons;
        public uint SampleRate;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DeviceInfoKeyboard
    {
        public uint Type;
        public uint SubType;
        public uint KeyboardMode;
        public uint NumberOfFunctionKeys;
        public uint NumberOfIndicators;
        public uint NumberOfKeysTotal;
        public bool IsUSBKeboard
        {
            get
            {
                return this.Type == 81;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DeviceInfoHID
    {
        public uint VendorID;
        public uint ProductID;
        public uint VersionNumber;
        public ushort UsagePage;
        public ushort Usage;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputDeviceList
    {
        public IntPtr DeviceHandle;
        public RawInputDeviceType DeviceType;
    }

    [Flags]
    internal enum RawInputDeviceFlags
    {
        None,
        Remove = 0x1,
        Exclude = 0x10,
        PageOnly = 0x20,
        NoLegacy = 0x30,
        InputSink = 0x100,
        CaptureMouse = 0x200,
        NoHotKeys = 0x200,
        AppKeys = 0x400,
        ExInputSink = 0x1000,
        DevNotify = 0x2000,
    }

    internal enum DataCommand
    {
        RID_INPUT = 0x10000003,
        RID_HEADER = 0x10000005,
    }


    [StructLayout(LayoutKind.Sequential)]
    internal struct Rawinputheader
    {
        public uint dwType;
        public uint dwSize;
        public IntPtr hDevice;
        public IntPtr wParam;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInput
    {
        public RawInputHeader Header;
        public RawInputData Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct RawInputData
    {
        [FieldOffset(0)]
        public RawMouse Mouse;
        [FieldOffset(0)]
        public RawKeyboard Keyboard;
        [FieldOffset(0)]
        public RawHID HID;
    }



    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputHeader
    {
        public RawInputDeviceType Type;
        public int Size;
        public IntPtr Device;
        public IntPtr Param;
        public static int RawInputHeaderSize = Marshal.SizeOf<RawInputHeader>();
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawHID
    {
        public int dwSizeHid;
        public int dwCount;
        // public IntPtr bRawData;
    }

    internal struct RawMouse
    {
        /*public RawMouseState Flags;
        private short Buttons;
        public RawInputMouseState ButtonFlags;
        public short ButtonData;
        public int RawButtons;
        public int LastX;
        public int LastY;
        public int ExtraInformation;
        public static readonly int Size = Marshal.SizeOf(typeof(RawMouse));*/
    }

    internal struct RawKeyboard
    {
        /*public short MakeCode;
        public RawInputKeyboardFlags Flags;
        private readonly short Reserved;
        public VirtualKeys VirtualKey;
        public WindowMessage Message;
        public int ExtraInformation;
        public static readonly int Size = Marshal.SizeOf(typeof(RawKeyboard));*/
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputDevice
    {
        public ushort usUsagePage;
        public ushort usUsage;
        public RawInputDeviceFlags dwFlags;
        public IntPtr hwndTarget;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HIDP_CAPS
    {
        public ushort Usage;
        public ushort UsagePage;
        public ushort InputReportByteLength;
        public ushort OutputReportByteLength;
        public ushort FeatureReportByteLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public ushort[] Reserved;
        public ushort NumberLinkCollectionNodes;
        public ushort NumberInputButtonCaps;
        public ushort NumberInputValueCaps;
        public ushort NumberInputDataIndices;
        public ushort NumberOutputButtonCaps;
        public ushort NumberOutputValueCaps;
        public ushort NumberOutputDataIndices;
        public ushort NumberFeatureButtonCaps;
        public ushort NumberFeatureValueCaps;
        public ushort NumberFeatureDataIndices;
    }
#pragma warning restore 0649

    public static class NativeMethods
    {

        public static void RegisterDeviceHook(IntPtr handle)
        {
            var subscription = new RawInputDevice[] {
                new RawInputDevice { usUsage = 0x04, usUsagePage = 0x01, dwFlags = RawInputDeviceFlags.InputSink, hwndTarget = handle },
                new RawInputDevice { usUsage = 0x05, usUsagePage = 0x01, dwFlags = RawInputDeviceFlags.InputSink, hwndTarget = handle },
                new RawInputDevice { usUsage = 0x08, usUsagePage = 0x01, dwFlags = RawInputDeviceFlags.InputSink, hwndTarget = handle },
            };
            var result = RegisterRawInputDevices(subscription, (uint)subscription.Length, (uint)Marshal.SizeOf<RawInputDevice>());
            if (!result)
            {
                throw new InvalidOperationException("Failed to register hid device hook");
            }
        }


        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint FILE_SHARE_WRITE = 0x2;
        public const uint FILE_SHARE_READ = 0x1;
        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        public const uint OPEN_EXISTING = 3;
        public const uint OPEN_ALWAYS = 4;
        private const int HIDP_STATUS_SUCCESS = 0x110000;

        internal static void ReadData(IntPtr hdevice)
        {
            IntPtr buffer = IntPtr.Zero;
            uint size = 0;
            GetRawInputData(hdevice, DataCommand.RID_INPUT, buffer, ref size, Marshal.SizeOf(typeof(Rawinputheader)));
            try
            {
                buffer = Marshal.AllocHGlobal((int)(size));
                if (GetRawInputData(hdevice, DataCommand.RID_INPUT, buffer, ref size, Marshal.SizeOf(typeof(Rawinputheader))) == size)
                {
                    var rawInput = (RawInput)Marshal.PtrToStructure(buffer, typeof(RawInput));
                    var hidData = rawInput.Data.HID;
                    int offset = Marshal.SizeOf(typeof(Rawinputheader)) + Marshal.SizeOf(typeof(RawHID));


                    byte[] inputBuffer = new byte[hidData.dwSizeHid * hidData.dwCount];
                    Marshal.Copy(buffer + offset, inputBuffer, 0, inputBuffer.Length);
                    var info = GetInfo(rawInput.Header.Device);
                    if (info.Name == "\\\\?\\HID#VID_046D&PID_C218#9&779538d&1&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}")
                    {
                        var handle = CreateFile(info.Name, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, IntPtr.Zero);
                        SafeFileHandle sHandle = new SafeFileHandle(handle, false);
                        IntPtr preparsedData = IntPtr.Zero;
                        if (HidD_GetPreparsedData(sHandle, ref preparsedData))
                        {
                            try
                            {
                                IntPtr capabilities = IntPtr.Zero;
                                var result = HidP_GetCaps(preparsedData, ref capabilities);
                                if (result != HIDP_STATUS_SUCCESS)
                                {
                                    throw new Exception($"Could not get Hid capabilities. Return code: {result}");
                                }
                                HIDP_CAPS hidCollectionCapabilities = (HIDP_CAPS)Marshal.PtrToStructure(capabilities, typeof(HIDP_CAPS));
                            }
                            finally
                            {
                                HidD_FreePreparsedData(ref preparsedData);
                            }
                        }
                        System.Diagnostics.Debug.WriteLine("GP" + string.Join(" ", inputBuffer.Select(b => b.ToString())));
                    }
                    if (info.Name == "\\\\?\\HID#VID_0810&PID_0001#9&27c934ab&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}")
                    {
                        System.Diagnostics.Debug.WriteLine("TW" + string.Join(" ", inputBuffer.Select(b => b.ToString())));
                    }
                }
            }
            finally
            {
                if (buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
        }

        internal static List<RawInputDeviceList> GetDeviceList()
        {
            uint deviceCount = 0;
            IntPtr deviceList = IntPtr.Zero;
            var deviceSize = (uint)Marshal.SizeOf(typeof(RawInputDeviceList));
            var result = GetRawInputDeviceList(deviceList, ref deviceCount, deviceSize);
            if ((int)result == -1)
            {
                throw new InvalidOperationException("Failed to get the number of hid devices");
            }
            if (deviceCount == 0)
            {
                return null;
            }

            try
            {
                deviceList = Marshal.AllocHGlobal((int)(deviceSize * deviceCount));
                result = GetRawInputDeviceList(deviceList, ref deviceCount, deviceSize);
                if ((int)result == -1)
                {
                    throw new InvalidOperationException("Failed to get the hid device list");
                }
                long basePointer;
                if (Environment.Is64BitProcess)
                {
                    basePointer = deviceList.ToInt64();
                }
                else
                {
                    basePointer = deviceList.ToInt32();
                }
                return Enumerable.Range(0, (int)deviceCount)
                    .Select(i =>
                    {
                        IntPtr elementPointer = new IntPtr(basePointer + deviceSize * i);
                        return (RawInputDeviceList)Marshal.PtrToStructure(elementPointer, typeof(RawInputDeviceList));
                    })
                    .ToList();
            }
            finally
            {
                if (deviceList != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(deviceList);
                }
            }
        }

        internal static Info GetInfo(IntPtr deviceHandle)
        {
            var deviceInfo = GetDeviceData(deviceHandle, InfoCommand.DeviceInfo, x => (DeviceInfo)Marshal.PtrToStructure(x, typeof(DeviceInfo)));
            string name = GetDeviceData(deviceHandle, InfoCommand.DeviceName, Marshal.PtrToStringAnsi);
            return new Info(name, deviceInfo.HIDInfo.VendorID, deviceInfo.HIDInfo.ProductID, deviceInfo.HIDInfo.VersionNumber, deviceInfo.HIDInfo.Usage, deviceInfo.HIDInfo.UsagePage);
        }

        internal static T GetDeviceData<T>(IntPtr deviceHandle, InfoCommand command, Func<IntPtr, T> dataGetter)
        {
            uint dataSize = 0;
            var data = IntPtr.Zero;
            GetRawInputDeviceInfo(deviceHandle, command, data, ref dataSize);
            if (dataSize == 0)
            {
                return default;
            }
            try
            {
                data = Marshal.AllocHGlobal((int)dataSize);
                var result = GetRawInputDeviceInfo(deviceHandle, command, data, ref dataSize);
                if ((int)result == -1)
                {
                    throw new InvalidOperationException("Failed to get the hid device info");
                }
                return dataGetter(data);
            }
            finally
            {
                if (data != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(data);
                }
            }
        }

        [DllImport("User32.dll", SetLastError = true)]
        static extern uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint uiNumDevices, uint cbSize);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetRawInputDeviceInfo(IntPtr hDevice, InfoCommand uiCommand, IntPtr data, ref uint size);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool RegisterRawInputDevices(RawInputDevice[] pRawInputDevices, uint uiNumDevices, uint size);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetRawInputData(IntPtr hRawInput, DataCommand uiCommand, IntPtr pData, ref uint pcbSize, int cbSizeHeader);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPStr)] string strName, uint nAccess, uint nShareMode, IntPtr lpSecurity, uint nCreationFlags, uint nAttributes, IntPtr lpTemplate);

        [DllImport("hid.dll", SetLastError = true)]
        static extern void HidD_GetHidGuid(out Guid Guid);

        [DllImport("hid.dll", SetLastError = true)]
        static extern bool HidD_GetPreparsedData(SafeFileHandle HidDeviceObject, ref IntPtr PreparsedData);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern bool HidD_FreePreparsedData(ref IntPtr pointerToPreparsedData);

        [DllImport("hid.dll", SetLastError = true)]
        private static extern int HidP_GetCaps(IntPtr pointerToPreparsedData, ref IntPtr Capabilities);

    }
}
