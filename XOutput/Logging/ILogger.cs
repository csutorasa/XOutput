using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Logging
{
    public interface ILogger
    {
        void Trace(string log);
        void Trace(Func<string> log);
        void Debug(string log);
        void Debug(Func<string> log);
        void Info(string log);
        void Info(Func<string> log);
        void Warning(string log);
        void Warning(Func<string> log);
        void Error(string log);
        void Error(Func<string> log);
        void Error(Exception ex);
    }
}
