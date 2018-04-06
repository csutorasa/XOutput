using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Logging
{
    public abstract class AbstractLogger : ILogger
    {
        private readonly Type loggerType;
        public Type LoggerType => loggerType;
        private readonly int level;
        public int Level => level;

        public AbstractLogger(Type loggerType, int level)
        {
            this.loggerType = loggerType;
            this.level = level;
        }

        protected string GetCallerMethodName()
        {
            MethodBase method = new StackTrace().GetFrame(2).GetMethod();
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

        protected string CreatePrefix(DateTime time, LogLevel loglevel, string classname, string methodname)
        {
            return $"{time.ToString("yyyy-MM-dd HH\\:mm\\:ss.fff zzz")} {loglevel.Text} {classname}.{methodname}: ";
        }

        public Task Trace(string log)
        {
            return LogCheck(LogLevel.Trace, GetCallerMethodName(), log);
        }

        public Task Trace(Func<string> log)
        {
            return LogCheck(LogLevel.Trace, GetCallerMethodName(), log);
        }

        public Task Debug(string log)
        {
            return LogCheck(LogLevel.Debug, GetCallerMethodName(), log);
        }

        public Task Debug(Func<string> log)
        {
            return LogCheck(LogLevel.Debug, GetCallerMethodName(), log);
        }

        public Task Info(string log)
        {
            return LogCheck(LogLevel.Info, GetCallerMethodName(), log);
        }

        public Task Info(Func<string> log)
        {
            return LogCheck(LogLevel.Info, GetCallerMethodName(), log);
        }

        public Task Warning(string log)
        {
            return LogCheck(LogLevel.Warning, GetCallerMethodName(), log);
        }

        public Task Warning(Func<string> log)
        {
            return LogCheck(LogLevel.Warning, GetCallerMethodName(), log);
        }

        public Task Error(string log)
        {
            return LogCheck(LogLevel.Error, GetCallerMethodName(), log);
        }

        public Task Error(Func<string> log)
        {
            return LogCheck(LogLevel.Error, GetCallerMethodName(), log);
        }

        public Task Error(Exception ex)
        {
            return LogCheck(LogLevel.Error, GetCallerMethodName(), ex.ToString());
        }

        protected Task LogCheck(LogLevel loglevel, string methodName, string log)
        {
            if (loglevel.Level >= Level)
            {
                return Log(loglevel, methodName, log);
            }
            return Task.Run(() => { });
        }

        protected Task LogCheck(LogLevel loglevel, string methodName, Func<string> log)
        {
            if (loglevel.Level >= Level)
            {
                return Log(loglevel, methodName, log());
            }
            return Task.Run(() => { });
        }

        protected abstract Task Log(LogLevel loglevel, string methodName, string log);
    }
}
