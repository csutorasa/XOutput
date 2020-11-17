using System;

namespace XOutput.App.Devices.Input
{
    public delegate void DeviceConnectedHandler(object sender, DeviceConnectedEventArgs e);

    public class DeviceConnectedEventArgs : EventArgs
    {
        public IInputDevice Device { get; private set; }

        public DeviceConnectedEventArgs(IInputDevice device)
        {
            Device = device;
        }
    }
}
