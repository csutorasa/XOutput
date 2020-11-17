using System;

namespace XOutput.App.Devices.Input
{
    public delegate void DeviceDisconnectedHandler(object sender, DeviceDisconnectedEventArgs e);

    public class DeviceDisconnectedEventArgs : EventArgs
    {
        public IInputDevice Device { get; private set; }

        public DeviceDisconnectedEventArgs(IInputDevice device)
        {
            Device = device;
        }
    }
}
