using System;

namespace XOutput.Core.DependencyInjection
{
    public class Dependency
    {
        public Type Type { get; set; }
        public bool Required { get; set; }
    }
}
