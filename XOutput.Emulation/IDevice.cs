using XOutput.Common;

namespace XOutput.Emulation
{
    public interface IDevice
    {
        event DeviceDisconnectedEventHandler Closed;
        string Id { get; }
        DeviceTypes DeviceType { get; }
        Emulators Emulator { get; }
        void Close();
    }

    public delegate void DeviceDisconnectedEventHandler(object sender, DeviceDisconnectedEventArgs args);

    public class DeviceDisconnectedEventArgs
    {

    }
}
