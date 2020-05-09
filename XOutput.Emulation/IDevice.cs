namespace XOutput.Emulation
{
    public interface IDevice
    {
        event DeviceDisconnectedEvent Closed;
        string Id { get; }
        DeviceTypes DeviceType { get; }
        void Close();
    }

    public delegate void DeviceDisconnectedEvent(object sender, DeviceDisconnectedEventArgs args);

    public class DeviceDisconnectedEventArgs
    {

    }
}
