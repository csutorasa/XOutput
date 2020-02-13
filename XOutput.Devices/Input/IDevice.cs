using System;
using System.Collections.Generic;

namespace XOutput.Devices.Input
{
    public interface IDevice : IDisposable
    {
        event DeviceInputChangedHandler InputChanged;
        event DeviceDisconnectedHandler Disconnected;
        IEnumerable<DPadDirection> DPads { get; }
        IEnumerable<InputSource> Sources { get; }
        IEnumerable<ForceFeedbackTarget> ForceFeedbacks { get; }
        InputConfig InputConfiguration { get; }
        string DisplayName { get; }
        string UniqueId { get; }
        string HardwareID { get; }
    }
}
