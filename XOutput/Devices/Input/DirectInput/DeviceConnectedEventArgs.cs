using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices.Input.DirectInput
{
    /// <summary>
    /// Event delegate for DeviceConnected event.
    /// </summary>
    /// <param name="sender">the connected <see cref="IInputDevice"/></param>
    /// <param name="e">event arguments</param>
    public delegate void DeviceConnectedHandler(object sender, DeviceConnectedEventArgs e);

    /// <summary>
    /// Event argument class for DeviceConnected event.
    /// </summary>
    public class DeviceConnectedEventArgs : EventArgs
    {

    }
}
