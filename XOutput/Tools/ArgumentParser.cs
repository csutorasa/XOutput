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

        private readonly bool minimized;
        /// <summary>
        /// Gets if the application should start in silent mode.
        /// </summary>
        public bool Minimized => minimized;

        public ArgumentParser() : this(Environment.GetCommandLineArgs().Skip(1).ToArray())
        {
            
        }

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
                logger.Warning($"Unused command line argument: {arg}");
            }
        }
    }
}
