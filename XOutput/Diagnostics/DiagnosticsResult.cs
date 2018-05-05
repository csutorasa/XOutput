using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Diagnostics
{
    public class DiagnosticsResult
    {
        public Enum Type { get; set; }
        public object Value { get; set; }
        public DiagnosticsResultState State { get; set; }
        public string Reason { get; set; }
    }
}
