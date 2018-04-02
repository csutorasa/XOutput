using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices.Input
{
    public delegate void DeviceDisconnectedHandler(object sender, DeviceDisconnectedEventArgs e);

    public class DeviceDisconnectedEventArgs : EventArgs
    {

    }
}
