namespace XOutput.Server.Emulation
{
    public interface IXboxEmulator : IEmulator
    {
        XboxDevice CreateXboxDevice();
    }
}
