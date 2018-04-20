using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Logging
{
    public interface ILogger
    {
        Task Trace(string log);
        Task Trace(Func<string> log);
        Task Debug(string log);
        Task Debug(Func<string> log);
        Task Info(string log);
        Task Info(Func<string> log);
        Task Warning(string log);
        Task Warning(Func<string> log);
        Task Error(string log);
        Task Error(Func<string> log);
        Task Error(Exception ex);
    }
}
