using System;
using System.Linq;
using XOutput.Core.DependencyInjection;
using XOutput.Tools;

namespace XOutput
{
    public static class ApplicationConfiguration
    {
        [ResolverMethod]
        public static ArgumentParser GetArgumentParser()
        {
            return new ArgumentParser(Environment.GetCommandLineArgs().Skip(1).ToArray());
        }
    }
}
