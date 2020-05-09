namespace XOutput.Emulation.Ds4
{
    public interface IDs4Emulator : IEmulator
    {
        Ds4Device CreateDs4Device();
    }
}
