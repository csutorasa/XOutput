using System;
using System.Threading;

namespace XOutput.Core.Threading
{
    public class ThreadTaskDefinition
    {
        public Action<CancellationToken> Task { get; set; }
        public Action Finally { get; set; }
        public Action<Exception> Error { get; set; }
        public Action Cancel { get; set; }
    }
}
