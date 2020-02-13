using System;

namespace XOutput.Devices.Input
{
    public delegate void DeviceDisconnectedHandler(object sender, DeviceDisconnectedEventArgs e);

    public class DeviceDisconnectedEventArgs : EventArgs
    {

    }
}
