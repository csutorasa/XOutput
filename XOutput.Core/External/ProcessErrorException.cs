using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

namespace XOutput.Core.External
{
    [Serializable]
    public sealed class ProcessErrorException : Exception
    {
        public Process Process { get; private set; }

        public ProcessErrorException() { }

        public ProcessErrorException(string message) : base(message) { }

        public ProcessErrorException(string message, Exception innerException) : base(message, innerException) { }

        public ProcessErrorException(Process process) : this($"Process failed with exit code {process.ExitCode}") {
            Process = process;
        }

        private ProcessErrorException(SerializationInfo info, StreamingContext context)
        {

        }
    }
}
