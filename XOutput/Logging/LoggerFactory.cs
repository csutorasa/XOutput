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
            return new TraceLogger(type);
        }
    }
}
