namespace XOutput.Rest.Devices
{
    public class DeviceInfo
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public string DeviceType { get; set; }
        public string Emulator { get; set; }
        public bool Active { get; set; }
        public bool Local { get; set; }
    }
}
