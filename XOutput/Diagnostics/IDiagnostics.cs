using System.Collections.Generic;

namespace XOutput.Diagnostics
{
    public interface IDiagnostics
    {
        /// <summary>
        /// Gets the source of the values.
        /// </summary>
        object Source { get; }
        /// <summary>
        /// Gets the result of the diagnostics.
        /// </summary>
        /// <returns>result</returns>
        IEnumerable<DiagnosticsResult> GetResults();
    }
}
