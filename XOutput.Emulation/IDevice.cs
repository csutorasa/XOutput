using XOutput.Common;

namespace XOutput.Emulation
{
    public interface IDevice
    {
        event DeviceDisconnectedEvent Closed;
        string Id { get; }
        DeviceTypes DeviceType { get; }
        Emulators Emulator { get; }
        void Close();
    }

    public delegate void DeviceDisconnectedEvent(object sender, DeviceDisconnectedEventArgs args);

    public class DeviceDisconnectedEventArgs
    {

    }
}
