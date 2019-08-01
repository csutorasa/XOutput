using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Logging;

namespace XOutput.Tools
{
    /// <summary>
    /// Parses command line arguments.
    /// </summary>
    public class ArgumentParser
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(ArgumentParser));

        private static ArgumentParser instance = new ArgumentParser();
        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        public static ArgumentParser Instance => instance;

        private readonly IEnumerable<string> startControllers;
        /// <summary>
        /// Gets the controller names to start additionally.
        /// </summary>
        public IEnumerable<string> StartControllers => startControllers;
        private readonly bool minimized;
        /// <summary>
        /// Gets if the application should start in silent mode.
        /// </summary>
        public bool Minimized => minimized;

        protected ArgumentParser()
        {
            var args = Environment.GetCommandLineArgs().ToList();
            args.RemoveAt(0);
            startControllers = args.Where(arg => arg.StartsWith("--start=")).Select(arg => arg.Replace("--start=", "")).ToArray();
            minimized = args.Any(arg => arg == "--minimized");
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
