using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Core.DependencyInjection
{
    public class Dependency
    {
        public Type Type { get; set; }
        public bool Required { get; set; }
    }
}
