using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Tools;
using XOutput.UI.Windows;

namespace XOutput
{
    public static class ApplicationConfiguration
    {
        [ResolverMethod]
        public static ArgumentParser GetArgumentParser()
        {
            return new ArgumentParser();
        }
        [ResolverMethod]
        public static HidGuardianManager GetHidGuardianManager(RegistryModifier registryModifier)
        {
            return new HidGuardianManager(registryModifier);
        }
        [ResolverMethod]
        public static RegistryModifier GetRegistryModifier()
        {
            return new RegistryModifier();
        }
    }
}
