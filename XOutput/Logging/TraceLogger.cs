using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Logging
{
    public class TraceLogger : AbstractLogger
    {
        public const string LogFile = "XOutput.log";

        static TraceLogger()
        {
            System.Diagnostics.Trace.AutoFlush = true;
            if (File.Exists(LogFile))
            {
                try
                {
                    File.Delete(LogFile);
                }
                catch { }
                System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(LogFile));
            }
        }

        public TraceLogger(Type loggerType, int level) : base(loggerType, level)
        {

        }

        protected override void Log(LogLevel loglevel, string methodName, string log)
        {
            System.Diagnostics.Trace.WriteLine(CreatePrefix(DateTime.Now, loglevel, LoggerType.Name, methodName) + log);
        }
    }
}
