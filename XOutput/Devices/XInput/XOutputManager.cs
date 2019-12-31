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


        public int Start()
        {
            if (!HasDevice)
            {
                return 0;
            }
            var controllerCount = Controllers.Instance.GetId();
            if (!xOutputDevice.Plugin(controllerCount))
            {
                ResetId(controllerCount);
            }
            return controllerCount;
        }

        private void ResetId(int controllerCount)
        {
            if (controllerCount != 0)
            {
                Controllers.Instance.DisposeId(controllerCount);
            }
        }

        public bool Stop(int controllerCount)
        {
            if (!HasDevice)
            {
                return false;
            }
            bool result = xOutputDevice.Unplug(controllerCount);
            if (result)
            {
                Controllers.Instance.DisposeId(controllerCount);
            }
            return result;
        }
    }
}
