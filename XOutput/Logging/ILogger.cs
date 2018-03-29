using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Logging
{
    public interface ILogger
    {
        void Info(string log, params string[] args);
        void Warning(string log, params string[] args);
        void Error(string log, params string[] args);
        void Error(Exception ex);
    }
}
