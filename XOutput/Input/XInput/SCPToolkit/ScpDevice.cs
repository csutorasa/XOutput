using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input.XInput
{
    public sealed class ScpDevice
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

        public bool Plugin(int controller, byte[] output = null)
        {
            byte[] buffer = new byte[8];
            return sendToDevice(NativeInterface.MessageType.Plugin, controller, buffer, output);
        }
        public bool Unplug(int controller, byte[] output = null)
        {
            byte[] buffer = new byte[8];
            return sendToDevice(NativeInterface.MessageType.Unplug, controller, buffer, output);
        }
        public bool UnplugAll(byte[] output = null)
        {
            byte[] buffer = new byte[8];
            return sendToDevice(NativeInterface.MessageType.Unplug, null, buffer, output);
        }

        public bool Report(int controller, byte[] input, byte[] output = null)
        {
            return sendToDevice(NativeInterface.MessageType.Report, controller, input, output);
        }
        
        private bool sendToDevice(NativeInterface.MessageType type, int? controller, byte[] input, byte[] output)
        {
            if (_safeFileHandle.IsInvalid || _safeFileHandle.IsClosed)
                return false;
            return NativeInterface.SendToDevice(_safeFileHandle, type, controller, input, output);
        }
    }
}
