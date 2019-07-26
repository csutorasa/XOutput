using System;

namespace XOutput.Devices.Input
{
    /// <summary>
    /// Event delegate for DeviceDisconnected event.
    /// </summary>
    /// <param name="sender">the disconnected <see cref="IInputDevice"/></param>
    /// <param name="e">event arguments</param>
    public delegate void DeviceDisconnectedHandler(object sender, DeviceDisconnectedEventArgs e);

    /// <summary>
    /// Event argument class for DeviceDisconnected event.
    /// </summary>
    public class DeviceDisconnectedEventArgs : EventArgs
    {

    }
}
