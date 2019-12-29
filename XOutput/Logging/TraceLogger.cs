using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace XOutput.Logging
{
    /// <summary>
    /// Writes logs with the help of <see cref="System.Diagnostics.Trace"/>
    /// </summary>
    public class TraceLogger : AbstractLogger
    {
        public const string LogFile = "XOutput.log";

        static TraceLogger()
        {
            System.Diagnostics.Trace.AutoFlush = true;
            System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(LogFile));
        }

        public TraceLogger(Type loggerType, int level) : base(loggerType, level)
        {

        }

        /// <summary>
        /// Writes the log.
        /// <para>Implements <see cref="AbstractLogger.LogCheck(LogLevel, string, string)"/></para>
        /// </summary>
        /// <param name="loglevel">loglevel</param>
        /// <param name="methodName">name of the caller method</param>
        /// <param name="log">log text</param>
        /// <returns></returns>
        protected override void Log(LogLevel loglevel, StackFrame stackFrame, string log)
        {
            System.Diagnostics.Trace.WriteLine(CreatePrefix(DateTime.Now, loglevel, LoggerType, stackFrame) + log);
        }
    }
}
