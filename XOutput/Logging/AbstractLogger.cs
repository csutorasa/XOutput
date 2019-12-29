using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace XOutput.Logging
{
    /// <summary>
    /// Logger base class.
    /// </summary>
    public abstract class AbstractLogger : ILogger
    {
        public const string LogFile = "XOutput.log";

        private readonly Type loggerType;
        public Type LoggerType => loggerType;
        private readonly int level;
        public int Level => level;

        static AbstractLogger() {
            
            if (File.Exists(LogFile))
            {
                try
                {
                    File.Delete(LogFile);
                }
                catch {
                    // if the file is in use, append the file
                }
            }
        }

        protected AbstractLogger(Type loggerType, int level)
        {
            this.loggerType = loggerType;
            this.level = level;
        }

        protected string GetCallerMethodName(StackFrame frame)
        {
            MethodBase method = frame.GetMethod();
            bool asyncFunction = method.DeclaringType.Name.Contains("<") && method.DeclaringType.Name.Contains(">");
            if (asyncFunction)
            {
                int openIndex = method.DeclaringType.Name.IndexOf("<");
                int closeIndex = method.DeclaringType.Name.IndexOf(">");
                return method.DeclaringType.Name.Substring(openIndex + 1, closeIndex - openIndex - 1);
            }
            else
            {
                return method.Name;
            }
        }

        protected string CreatePrefix(DateTime time, LogLevel loglevel, Type clazz, StackFrame stackFrame)
        {
            string methodName = GetCallerMethodName(stackFrame);
            return $"{time.ToString("yyyy-MM-dd HH\\:mm\\:ss.fff zzz")} {loglevel.Text} {clazz.FullName}.{methodName}: ";
        }

        public void Trace(string log)
        {
            LogCheck(LogLevel.Trace, new StackTrace().GetFrame(1), log);
        }

        public void Trace(Func<string> log)
        {
            LogCheck(LogLevel.Trace, new StackTrace().GetFrame(1), log);
        }

        public void Debug(string log)
        {
            LogCheck(LogLevel.Debug, new StackTrace().GetFrame(1), log);
        }

        public void Debug(Func<string> log)
        {
            LogCheck(LogLevel.Debug, new StackTrace().GetFrame(1), log);
        }

        public void Info(string log)
        {
            LogCheck(LogLevel.Info, new StackTrace().GetFrame(1), log);
        }

        public void Info(Func<string> log)
        {
            LogCheck(LogLevel.Info, new StackTrace().GetFrame(1), log);
        }

        public void Warning(string log)
        {
            LogCheck(LogLevel.Warning, new StackTrace().GetFrame(1), log);
        }

        public void Warning(Func<string> log)
        {
            LogCheck(LogLevel.Warning, new StackTrace().GetFrame(1), log);
        }

        public void Warning(Exception ex)
        {
            LogCheck(LogLevel.Warning, new StackTrace().GetFrame(1), ex.ToString());
        }

        public void Warning(string log, Exception ex)
        {
            LogCheck(LogLevel.Warning, new StackTrace().GetFrame(1), log, ex);
        }

        public void Warning(Func<string> log, Exception ex)
        {
            LogCheck(LogLevel.Warning, new StackTrace().GetFrame(1), log, ex);
        }

        public void Error(string log)
        {
            LogCheck(LogLevel.Error, new StackTrace().GetFrame(1), log);
        }

        public void Error(Func<string> log)
        {
            LogCheck(LogLevel.Error, new StackTrace().GetFrame(1), log);
        }

        public void Error(Exception ex)
        {
            LogCheck(LogLevel.Error, new StackTrace().GetFrame(1), ex.ToString());
        }

        public void Error(string log, Exception ex)
        {
            LogCheck(LogLevel.Error, new StackTrace().GetFrame(1), log, ex);
        }

        public void Error(Func<string> log, Exception ex)
        {
            LogCheck(LogLevel.Error, new StackTrace().GetFrame(1), log, ex);
        }

        protected void LogCheck(LogLevel loglevel, StackFrame stackFrame, string log)
        {
            if (loglevel.Level >= Level)
            {
                Log(loglevel, stackFrame, log);
            }
        }

        protected void LogCheck(LogLevel loglevel, StackFrame stackFrame, string log, Exception ex)
        {
            if (loglevel.Level >= Level)
            {
                Log(loglevel, stackFrame, log + Environment.NewLine + ex.ToString());
            }
        }

        protected void LogCheck(LogLevel loglevel, StackFrame stackFrame, Func<string> log)
        {
            if (loglevel.Level >= Level)
            {
                Log(loglevel, stackFrame, log());
            }
        }

        protected void LogCheck(LogLevel loglevel, StackFrame stackFrame, Func<string> log, Exception ex)
        {
            if (loglevel.Level >= Level)
            {
                Log(loglevel, stackFrame, log() + Environment.NewLine + ex.ToString());
            }
        }

        public void Log(string log, LogLevel level)
        {
            LogCheck(level, new StackTrace().GetFrame(1), log);
        }

        public void Log(string log, Exception ex, LogLevel level)
        {
            LogCheck(level, new StackTrace().GetFrame(1), log, ex);
        }

        public void Log(Func<string> log, LogLevel level)
        {
            LogCheck(level, new StackTrace().GetFrame(1), log());
        }

        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="loglevel">loglevel</param>
        /// <param name="methodName">name of the caller method</param>
        /// <param name="log">log text</param>
        /// <returns></returns>
        protected abstract void Log(LogLevel loglevel, StackFrame stackFrame, string log);

        public void SafeCall(Action action)
        {
            SafeCall(action, null, LogLevel.Error, new StackTrace().GetFrame(1));
        }
        public void SafeCall(Action action, string log)
        {
            SafeCall(action, log, LogLevel.Error, new StackTrace().GetFrame(1));
        }

        public void SafeCall(Action action, string log, LogLevel level)
        {
            SafeCall(action, log, level, new StackTrace().GetFrame(1));
        }

        public T SafeCall<T>(Func<T> action)
        {
            return SafeCall(action, null, LogLevel.Error, new StackTrace().GetFrame(1));
        }

        public T SafeCall<T>(Func<T> action, string log)
        {
            return SafeCall(action, log, LogLevel.Error, new StackTrace().GetFrame(1));
        }

        public T SafeCall<T>(Func<T> action, string log, LogLevel level)
        {
            return SafeCall(action, log, level, new StackTrace().GetFrame(1));
        }

        private void SafeCall(Action action, string log, LogLevel level, StackFrame stackFrame)
        {
            SafeCall<object>(() =>
            {
                action();
                return null;
            }, log, level, stackFrame);
        }

        private T SafeCall<T>(Func<T> action, string log, LogLevel level, StackFrame stackFrame)
        {
            try
            {
                return action();
            }
            catch(Exception ex)
            {
                Log(level, stackFrame, log == null ? ex.ToString() : log + Environment.NewLine + ex.ToString());
                return default;
            }
        }
    }
}
