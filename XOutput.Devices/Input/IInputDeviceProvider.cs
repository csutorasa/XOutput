using System;
using System.Collections.Generic;

namespace XOutput.Devices.Input
{
    public interface IInputDeviceProvider : IInputConfigManager, IDisposable
    {
        event DeviceConnectedHandler Connected;
        event DeviceDisconnectedHandler Disconnected;

        void SearchDevices();
        IEnumerable<IInputDevice> GetActiveDevices();
        bool SaveInputConfig(string id, InputConfig config);
    }
}
