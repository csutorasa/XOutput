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

        public void Trace(string log)
        {
            LogCheck(LogLevel.Trace, GetCallerMethodName(), log);
        }

        public void Trace(Func<string> log)
        {
            LogCheck(LogLevel.Trace, GetCallerMethodName(), log);
        }

        public void Debug(string log)
        {
            LogCheck(LogLevel.Debug, GetCallerMethodName(), log);
        }

        public void Debug(Func<string> log)
        {
            LogCheck(LogLevel.Debug, GetCallerMethodName(), log);
        }

        public void Info(string log)
        {
            LogCheck(LogLevel.Info, GetCallerMethodName(), log);
        }

        public void Info(Func<string> log)
        {
            LogCheck(LogLevel.Info, GetCallerMethodName(), log);
        }

        public void Warning(string log)
        {
            LogCheck(LogLevel.Warning, GetCallerMethodName(), log);
        }

        public void Warning(Func<string> log)
        {
            LogCheck(LogLevel.Warning, GetCallerMethodName(), log);
        }

        public void Error(string log)
        {
            LogCheck(LogLevel.Error, GetCallerMethodName(), log);
        }

        public void Error(Func<string> log)
        {
            LogCheck(LogLevel.Error, GetCallerMethodName(), log);
        }

        public void Error(Exception ex)
        {
            LogCheck(LogLevel.Error, GetCallerMethodName(), ex.ToString());
        }

        protected void LogCheck(LogLevel loglevel, string methodName, string log)
        {
            if (loglevel.Level >= Level)
            {
                Log(loglevel, methodName, log);
            }
        }

        protected void LogCheck(LogLevel loglevel, string methodName, Func<string> log)
        {
            if (loglevel.Level >= Level)
            {
                Log(loglevel, methodName, log());
            }
        }

        protected abstract void Log(LogLevel loglevel, string methodName, string log);
    }
}
