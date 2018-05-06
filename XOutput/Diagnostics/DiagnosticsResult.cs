using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
