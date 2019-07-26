using System;

namespace XOutput.Logging
{
    public static class LoggerFactory
    {
        /// <summary>
        /// Gets a new logger for the <paramref name="type"/>.
        /// Uses Debug level if debug built is done.
        /// </summary>
        /// <param name="type">class type</param>
        /// <returns></returns>
        public static ILogger GetLogger(Type type)
        {
#if DEBUG
            return GetLogger(type, LogLevel.Debug.Level);
#else
            return GetLogger(type, LogLevel.Info.Level);
#endif
        }

        /// <summary>
        /// Gets a new logger for the <paramref name="type"/>.
        /// </summary>
        /// <param name="type">class type</param>
        /// <param name="level">predefined loglevel</param>
        /// <returns></returns>
        public static ILogger GetLogger(Type type, int level)
        {
            return new TraceLogger(type, level);
        }
    }
}
