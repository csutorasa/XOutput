
using System.Collections.Generic;
using System.Linq;
using XOutput.Api.Devices;
using XOutput.Core.DependencyInjection;

namespace XOutput.Server.Emulation
{
    public class EmulatorService
    {
        private readonly List<IEmulator> emulators;

        [ResolverMethod]
        public EmulatorService(ApplicationContext applicationContext)
        {
            emulators = applicationContext.ResolveAll<IEmulator>();
        }

        public T FindEmulator<T>(DeviceTypes deviceType, string emulator) where T : IEmulator
        {
            return emulators
                .Where(e => e.Name == emulator)
                .Where(e => e.SupportedDeviceTypes.Contains(deviceType))
                .OfType<T>()
                .First();
        }

        public List<IEmulator> GetEmulators()
        {
            return emulators.ToList();
        }

    }
}
