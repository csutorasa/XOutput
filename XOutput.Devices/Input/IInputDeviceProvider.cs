using System;
using System.Collections.Generic;

namespace XOutput.Devices.Input
{
    public interface IInputDeviceProvider : IDisposable
    {
        event DeviceConnectedHandler Connected;
        event DeviceDisconnectedHandler Disconnected;
        void SearchDevices();
        IEnumerable<IInputDevice> GetActiveDevices();
    }
}
