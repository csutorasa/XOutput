using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Core.Configuration;
using XOutput.Tools;
using XOutput.UI.Windows;

namespace XOutput
{
    public static class ApplicationConfiguration
    {
        [ResolverMethod]
        public static ArgumentParser GetArgumentParser()
        {
            return new ArgumentParser(Environment.GetCommandLineArgs().Skip(1).ToArray());
        }

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
