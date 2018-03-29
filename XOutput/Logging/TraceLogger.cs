using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Logging
{
    public class TraceLogger : AbstractLogger
    {
        public const string LOG_FILE = "XOutput.log";

        static TraceLogger()
        {
            Trace.AutoFlush = true;
            Trace.Listeners.Add(new TextWriterTraceListener(LOG_FILE));
        }

        public TraceLogger(Type loggerType) : base(loggerType)
        {

        }

        public override void Error(string log, params string[] args)
        {
            Trace.TraceError(CreatePrefix(DateTime.Now, loggerType.Name, GetCallerMethodName()) + log, args);
        }

        public override void Error(Exception ex)
        {
            Trace.TraceError(CreatePrefix(DateTime.Now, loggerType.Name, GetCallerMethodName()) + ex.ToString());
        }

        public override void Info(string log, params string[] args)
        {
            Trace.TraceInformation(CreatePrefix(DateTime.Now, loggerType.Name, GetCallerMethodName()) + log, args);
        }

        public override void Warning(string log, params string[] args)
        {
            Trace.TraceWarning(CreatePrefix(DateTime.Now, loggerType.Name, GetCallerMethodName()) + log, args);
        }
    }
}
