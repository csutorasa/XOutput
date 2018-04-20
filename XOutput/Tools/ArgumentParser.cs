using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Logging;

namespace XOutput.Tools
{
    public class ArgumentParser
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(ArgumentParser));

        private static ArgumentParser instance = new ArgumentParser();
        public static ArgumentParser Instance => instance;

        private readonly IEnumerable<string> startControllers;
        public IEnumerable<string> StartControllers => startControllers;
        private readonly bool minimized;
        public bool Minimized => minimized;

        protected ArgumentParser()
        {
            var args = Environment.GetCommandLineArgs().ToList();
            args.RemoveAt(0);
            startControllers = args.Where(arg => arg.StartsWith("--start=")).Select(arg => arg.Replace("--start=", "")).ToArray();
            minimized = args.Where(arg => arg == "--minimized").Any();
            if (minimized)
            {
                args.Remove("--minimized");
            }
            foreach (var arg in args)
            {
                logger.Warning($"Unused command line argument: {arg}");
            }
        }
    }
}
