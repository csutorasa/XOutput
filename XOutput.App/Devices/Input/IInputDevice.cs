using System;
using System.Collections.Generic;

namespace XOutput.App.Devices.Input
{
    public interface IInputDevice : IDisposable
    {
        event DeviceInputChangedHandler InputChanged;
        IEnumerable<InputSource> Sources { get; }
        IEnumerable<ForceFeedbackTarget> ForceFeedbacks { get; }
        InputConfig InputConfiguration { get; set; }
        InputDeviceMethod InputMethod { get; }
        string DisplayName { get; }
        string InterfacePath { get; }
        string UniqueId { get; }
        string HardwareID { get; }
        bool Running { get; }
        void Start();
        void Stop();
        InputSource FindSource(int offset);
        ForceFeedbackTarget FindTarget(int offset);
    }
}
