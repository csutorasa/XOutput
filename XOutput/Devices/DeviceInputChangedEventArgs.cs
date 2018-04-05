using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices
{
    public delegate void DeviceInputChangedHandler(object sender, DeviceInputChangedEventArgs e);

    public class DeviceInputChangedEventArgs : EventArgs
    {

    }
}
