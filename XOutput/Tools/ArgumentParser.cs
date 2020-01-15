using NLog;
using System.Collections.Generic;
using System.Linq;

namespace XOutput.Tools
{
    /// <summary>
    /// Parses command line arguments.
    /// </summary>
    public class ArgumentParser
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly bool minimized;
        /// <summary>
        /// Gets if the application should start in silent mode.
        /// </summary>
        public bool Minimized => minimized;

        public ArgumentParser(IEnumerable<string> arguments)
        {
            var args = arguments.ToList();
            minimized = args.Any(arg => arg == "--minimized");
            if (minimized)
            {
                args.Remove("--minimized");
            }
            foreach (var arg in args)
            {
                logger.Warn($"Unused command line argument: {arg}");
            }
        }
    }
}
