using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
            if (File.Exists(LogFile))
            {
                try
                {
                    File.Delete(LogFile);
                }
                catch { }
            }
            System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(LogFile));
        }

        public Task currentTask;

        public TraceLogger(Type loggerType, int level) : base(loggerType, level)
        {
            currentTask = Task.Run(() => { });
        }

        /// <summary>
        /// Writes the log.
        /// <para>Implements <see cref="AbstractLogger.LogCheck(LogLevel, string, string)"/></para>
        /// </summary>
        /// <param name="loglevel">loglevel</param>
        /// <param name="methodName">name of the caller method</param>
        /// <param name="log">log text</param>
        /// <returns></returns>
        protected override Task Log(LogLevel loglevel, string methodName, string log)
        {
            currentTask = currentTask.ContinueWith((t) => DoLog(loglevel, methodName, log));
            return currentTask;
        }

        private void DoLog(LogLevel loglevel, string methodName, string log)
        {
            System.Diagnostics.Trace.WriteLine(CreatePrefix(DateTime.Now, loglevel, LoggerType.FullName, methodName) + log);
        }
    }
}
