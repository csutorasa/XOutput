using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;

namespace XOutput.Core
{
    public static class CoreConfiguration
    {
        [ResolverMethod]
        public static FileManager GetFileManager()
        {
            return new FileManager();
        }

        [ResolverMethod]
        public static IConfigurationManager GetConfigurationManager(FileManager fileManager)
        {
            return new JsonConfigurationManager(fileManager);
        }
    }
}
