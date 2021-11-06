
using System.Collections.Generic;
using System.Linq;
using XOutput.Common;
using XOutput.DependencyInjection;
using XOutput.Emulation.Ds4;
using XOutput.Emulation.Xbox;

namespace XOutput.Emulation
{
    public class EmulatorService
    {
        private readonly List<IEmulator> emulators;

        [ResolverMethod]
        public EmulatorService(List<IEmulator> emulators)
        {
            this.emulators = emulators;
        }

        public T FindEmulator<T>(DeviceTypes deviceType, Emulators emulator) where T : IEmulator
        {
            return emulators
                .Where(e => e.Emulator == emulator)
                .Where(e => e.SupportedDeviceTypes.Contains(deviceType))
                .OfType<T>()
                .First();
        }

        public IXboxEmulator FindBestXboxEmulator()
        {
            var vigem = emulators.Find(emulator => emulator.Installed && emulator.Emulator == Emulators.ViGEm);
            if (vigem != null) {
                return (IXboxEmulator) vigem;
            }
            return (IXboxEmulator) emulators.Find(emulator => emulator.Installed && emulator.Emulator == Emulators.SCPToolkit);
        }

        public IDs4Emulator FindBestDs4Emulator()
        {
            return (IDs4Emulator) emulators.Find(emulator => emulator.Installed && emulator.Emulator == Emulators.ViGEm);
        }

        public List<IEmulator> GetEmulators()
        {
            return emulators.ToList();
        }

    }
}
