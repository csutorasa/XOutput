using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace XOutput.Logging
{
    public class FileLogger : AbstractLogger
    {
        public FileLogger(Type loggerType, int level) : base(loggerType, level)
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
            File.AppendAllText(LogFile, CreatePrefix(DateTime.Now, loglevel, LoggerType, stackFrame) + log + Environment.NewLine);
        }
    }
}
