using System;
using System.Collections.Generic;

namespace XOutput.App.Devices.Input
{
    public interface IInputDeviceProvider : IDisposable
    {
        event DeviceConnectedHandler Connected;
        event DeviceDisconnectedHandler Disconnected;
        void SearchDevices();
        IEnumerable<IInputDevice> GetActiveDevices();
    }
}
