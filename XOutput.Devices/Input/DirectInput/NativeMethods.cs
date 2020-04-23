using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace XOutput.Devices.Input.DirectInput
{
    static class NativeMethods
    {
        public static string GetHid(string vendor, string product)
        {
            int vendorId = Convert.ToInt32(vendor, 16);
            int productId = Convert.ToInt32(product, 16);
            return GetHid(vendorId, productId);
        }

        public static string GetHid(int vendorId, int productId)
        {
            var device = GetDeviceList()
                .Where(d => d.DeviceType == RawInputDeviceType.HumanInterfaceDevice)
                .Select(d => d.DeviceHandle)
                .Select(GetInfo)
                .FirstOrDefault(i => i.VendorId == vendorId && i.ProductId == productId);
            return device?.ToHidString();
        }

        private static List<RawInputDeviceList> GetDeviceList()
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

        private static Info GetInfo(IntPtr deviceHandle)
        {
            var deviceInfo = GetDeviceData(deviceHandle, InfoCommand.DeviceInfo, x => (DeviceInfo)Marshal.PtrToStructure(x, typeof(DeviceInfo)));
            string name = GetDeviceData(deviceHandle, InfoCommand.DeviceName, Marshal.PtrToStringAnsi);
            return new Info(name, deviceInfo.HIDInfo.VendorID, deviceInfo.HIDInfo.ProductID, deviceInfo.HIDInfo.VersionNumber);
        }

        private static T GetDeviceData<T>(IntPtr deviceHandle, InfoCommand command, Func<IntPtr, T> dataGetter)
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

        class Info
        {
            public string Name { get; private set; }
            public uint VendorId { get; private set; }
            public uint ProductId { get; private set; }
            public uint VersionNumber { get; private set; }

            public Info(string name, uint vendorID, uint productId, uint versionNumber)
            {
                Name = name;
                VendorId = vendorID;
                ProductId = productId;
                VersionNumber = versionNumber;
            }

            internal string ToHidString()
            {
                string vendor = VendorId.ToString("X4");
                string product = ProductId.ToString("X4");
                string version = VersionNumber.ToString("X4");
                return $"HID\\VID_{vendor}&PID_{product}&REV_{version}";
            }
        }

#pragma warning disable 0649
        enum RawInputDeviceType : uint
        {
            Mouse = 0,
            Keyboard = 1,
            HumanInterfaceDevice = 2
        }

        enum InfoCommand : uint
        {
            PreparsedData = 0x20000005,
            DeviceName = 0x20000007,
            DeviceInfo = 0x2000000b,
        }

        [StructLayout(LayoutKind.Explicit)]
        struct DeviceInfo
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

        struct DeviceInfoMouse
        {
            public uint ID;
            public uint NumberOfButtons;
            public uint SampleRate;
        }

        struct DeviceInfoKeyboard
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

        struct DeviceInfoHID
        {
            public uint VendorID;
            public uint ProductID;
            public uint VersionNumber;
            public ushort UsagePage;
            public ushort Usage;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RawInputDeviceList
        {
            public IntPtr DeviceHandle;
            public RawInputDeviceType DeviceType;
        }

        [DllImport("User32.dll", SetLastError = true)]
        static extern uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint uiNumDevices, uint cbSize);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetRawInputDeviceInfo(IntPtr hDevice, InfoCommand uiCommand, IntPtr data, ref uint size);
#pragma warning restore 0649
    }
}
