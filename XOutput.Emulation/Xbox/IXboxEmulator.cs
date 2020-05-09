namespace XOutput.Emulation.Xbox
{
    public interface IXboxEmulator : IEmulator
    {
        XboxDevice CreateXboxDevice();
    }
}
