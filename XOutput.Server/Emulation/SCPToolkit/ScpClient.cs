using Microsoft.Win32.SafeHandles;
using System;
using System.IO;

namespace XOutput.Server.Emulation.SCPToolkit
{
    public sealed class ScpClient : IDisposable
    {
        /// <summary>
        /// SCP Bus class GUID
        /// </summary>
        private const string SCPBusClassGUID = "{F679F562-3164-42CE-A4DB-E7DDBE723909}";

        private readonly SafeFileHandle safeFileHandle;
        private bool disposed = false;

        public ScpClient() : this(0) { }
        public ScpClient(int instance)
        {
            string devicePath = "";
            if (NativeMethods.Find(new Guid(SCPBusClassGUID), ref devicePath, instance))
            {
                safeFileHandle = NativeMethods.GetHandle(devicePath);
            }
            else
            {
                throw new IOException("SCP Device cannot be found");
            }
        }
        public ScpClient(string devicePath)
        {
            safeFileHandle = NativeMethods.GetHandle(devicePath);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return; 
            
            if (disposing) {
                if (safeFileHandle != null && !safeFileHandle.IsInvalid)
                {
                    safeFileHandle.Dispose();
                }
            }
            
            disposed = true;
        }

        public void Plugin(int controllerCount)
        {
            byte[] buffer = new byte[8];
            SendToDevice(NativeMethods.MessageType.Plugin, controllerCount, buffer, null);
        }

        public void Unplug(int controllerCount)
        {
            byte[] buffer = new byte[8];
            SendToDevice(NativeMethods.MessageType.Unplug, controllerCount, buffer, null);
        }

        public void UnplugAll()
        {
            byte[] buffer = new byte[8];
            SendToDevice(NativeMethods.MessageType.Unplug, null, buffer, null);
        }

        public void Report(int controllerCount, byte[] report)
        {
            SendToDevice(NativeMethods.MessageType.Report, controllerCount, report, null);
        }

        private void SendToDevice(NativeMethods.MessageType type, int? controller, byte[] input, byte[] output)
        {
            if (safeFileHandle.IsInvalid || safeFileHandle.IsClosed)
            {
                throw new InvalidOperationException("File handle is closed or invalid");
            }
            bool success = NativeMethods.SendToDevice(safeFileHandle, type, controller, input, output);
            if (!success)
            {
                throw new InvalidOperationException("Failed to send message to device with type " + type.ToString());
            }
        }
    }
}
