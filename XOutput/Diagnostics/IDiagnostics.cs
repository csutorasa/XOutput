using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Diagnostics
{
    public interface IDiagnostics
    {
        object Source { get; }
        IEnumerable<DiagnosticsResult> GetResults();
    }
}
