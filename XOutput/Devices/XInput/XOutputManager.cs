using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.XInput.SCPToolkit;
using XOutput.Devices.XInput.Vigem;
using XOutput.Tools;

namespace XOutput.Devices.XInput
{
    public class XOutputManager
    {
        private readonly IXOutputInterface xOutputDevice;

        public IXOutputInterface XOutputDevice => xOutputDevice;

        public bool HasDevice => xOutputDevice != null;

        public bool IsVigem => xOutputDevice is VigemDevice;

        public bool IsScp => xOutputDevice is ScpDevice;

        [ResolverMethod]
        public XOutputManager()
        {
            if (VigemDevice.IsAvailable())
            {
                xOutputDevice = new VigemDevice();
            }
            else if (ScpDevice.IsAvailable())
            {
                xOutputDevice = new ScpDevice();
            } else
            {
                xOutputDevice = null;
            }
        }
    }
}
