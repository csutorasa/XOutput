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
        protected readonly Type loggerType;

        public AbstractLogger(Type loggerType)
        {
            this.loggerType = loggerType;
        }

        protected MethodBase GetCallerMethod()
        {
            return new StackTrace().GetFrame(2).GetMethod();
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

        protected string CreatePrefix(DateTime time, string classname, string methodname)
        {
            return $"{time.ToString("yyyy-MM-dd HH\\:mm\\:ss.fff zzz")} {classname}.{methodname}: ";
        }

        public abstract void Error(string log, params string[] args);
        public abstract void Error(Exception ex);
        public abstract void Info(string log, params string[] args);
        public abstract void Warning(string log, params string[] args);
    }
}
