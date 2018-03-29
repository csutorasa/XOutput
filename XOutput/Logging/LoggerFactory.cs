using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Logging
{
    public static class LoggerFactory
    {
        public static ILogger GetLogger(Type type)
        {
#if DEBUG
            return GetLogger(type, LogLevel.Debug.Level);
#else
            return GetLogger(type, LogLevel.Info.Level);
#endif
        }

        public static ILogger GetLogger(Type type, int level)
        {
            return new TraceLogger(type, level);
        }
    }
}
