using System;

namespace XOutput.Diagnostics
{
    public class DiagnosticsResult
    {
        /// <summary>
        /// Gets or sets the type of the diagnostic.
        /// </summary>
        public Enum Type { get; set; }
        /// <summary>
        /// Gets or sets the result of the diagnostic.
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Gets or sets the result state of the diagnostic.
        /// </summary>
        public DiagnosticsResultState State { get; set; }
    }
}
