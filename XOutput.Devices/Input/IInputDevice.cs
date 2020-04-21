using System;
using System.Collections.Generic;

namespace XOutput.Devices.Input
{
    public interface IInputDevice : IDisposable
    {
        event DeviceInputChangedHandler InputChanged;
        IEnumerable<InputSource> Sources { get; }
        IEnumerable<ForceFeedbackTarget> ForceFeedbacks { get; }
        InputConfig InputConfiguration { get; }
        string DisplayName { get; }
        string UniqueId { get; }
        string HardwareID { get; }
        InputSource FindSource(int offset);
        ForceFeedbackTarget FindTarget(int offset);
    }
}
