using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input.XInput.SCPToolkit
{
    public sealed class ScpDevice : IXOutput
    {
        private const string SCP_BUS_CLASS_GUID = "{F679F562-3164-42CE-A4DB-E7DDBE723909}";

        private readonly SafeFileHandle _safeFileHandle;

        public ScpDevice() : this(0) { }
        public ScpDevice(int instance)
        {
            string devicePath = "";
            if (NativeInterface.Find(new Guid(SCP_BUS_CLASS_GUID), ref devicePath, instance))
                _safeFileHandle = NativeInterface.GetHandle(devicePath);
            else
                throw new IOException("SCP Device cannot be found");
        }
        public ScpDevice(string devicePath)
        {
            _safeFileHandle = NativeInterface.GetHandle(devicePath);
        }

        public static bool IsAvailable()
        {
            string devicePath = "";
            return NativeInterface.Find(new Guid(SCP_BUS_CLASS_GUID), ref devicePath, 0);
        }

        ~ScpDevice()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_safeFileHandle != null && !_safeFileHandle.IsInvalid)
            {
                _safeFileHandle.Dispose();
            }
        }

        public void Close()
        {
            Dispose();
        }

        public bool Plugin(int controller)
        {
            byte[] buffer = new byte[8];
            return sendToDevice(NativeInterface.MessageType.Plugin, controller, buffer, null);
        }

        public bool Unplug(int controller)
        {
            byte[] buffer = new byte[8];
            return sendToDevice(NativeInterface.MessageType.Unplug, controller, buffer, null);
        }

        public bool UnplugAll()
        {
            byte[] buffer = new byte[8];
            return sendToDevice(NativeInterface.MessageType.Unplug, null, buffer, null);
        }

        public bool Report(int controller, Dictionary<XInputTypes, double> values)
        {
            return sendToDevice(NativeInterface.MessageType.Report, controller, getBytes(values), null);
        }

        private bool sendToDevice(NativeInterface.MessageType type, int? controller, byte[] input, byte[] output)
        {
            if (_safeFileHandle.IsInvalid || _safeFileHandle.IsClosed)
                return false;
            return NativeInterface.SendToDevice(_safeFileHandle, type, controller, input, output);
        }


        /// <summary>
        /// Gets binary data to report to scp device.
        /// </summary>
        /// <returns></returns>
        private byte[] getBytes(Dictionary<XInputTypes, double> values)
        {
            byte[] report = new byte[20];
            report[0] = 0; // Input report
            report[1] = 20; // Message length

            // Buttons
            if (values[XInputTypes.UP] > 0.5) report[2] |= 1 << 0;
            if (values[XInputTypes.DOWN] > 0.5) report[2] |= 1 << 1;
            if (values[XInputTypes.LEFT] > 0.5) report[2] |= 1 << 2;
            if (values[XInputTypes.RIGHT] > 0.5) report[2] |= 1 << 3;
            if (values[XInputTypes.Start] > 0.5) report[2] |= 1 << 4;
            if (values[XInputTypes.Back] > 0.5) report[2] |= 1 << 5;
            if (values[XInputTypes.L3] > 0.5) report[2] |= 1 << 6;
            if (values[XInputTypes.R3] > 0.5) report[2] |= 1 << 7;

            if (values[XInputTypes.L1] > 0.5) report[3] |= 1 << 0;
            if (values[XInputTypes.R1] > 0.5) report[3] |= 1 << 1;
            if (values[XInputTypes.Home] > 0.5) report[3] |= 1 << 2;

            if (values[XInputTypes.A] > 0.5) report[3] |= 1 << 4;
            if (values[XInputTypes.B] > 0.5) report[3] |= 1 << 5;
            if (values[XInputTypes.X] > 0.5) report[3] |= 1 << 6;
            if (values[XInputTypes.Y] > 0.5) report[3] |= 1 << 7;

            // Axes
            byte l2 = (byte)(values[XInputTypes.L2] * byte.MaxValue);
            report[4] = l2;
            byte r2 = (byte)(values[XInputTypes.R2] * byte.MaxValue);
            report[5] = r2;

            ushort lx = (ushort)((values[XInputTypes.LX] - 0.5) * ushort.MaxValue);
            report[6] = (byte)(lx & 0xFF);
            report[7] = (byte)((lx >> 8) & 0xFF);
            ushort ly = (ushort)((values[XInputTypes.LY] - 0.5) * ushort.MaxValue);
            report[8] = (byte)(ly & 0xFF);
            report[9] = (byte)((ly >> 8) & 0xFF);

            ushort rx = (ushort)((values[XInputTypes.RX] - 0.5) * ushort.MaxValue);
            report[10] = (byte)(rx & 0xFF);
            report[11] = (byte)((rx >> 8) & 0xFF);
            ushort ry = (ushort)((values[XInputTypes.RY] - 0.5) * ushort.MaxValue);
            report[12] = (byte)(ry & 0xFF);
            report[13] = (byte)((ry >> 8) & 0xFF);

            return report;
        }
    }
}
