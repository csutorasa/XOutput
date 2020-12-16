using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Mapping.Input
{
    public class MappableSource
    {
        public int Id => id;
        public double Value { get; set; }

        private readonly int id;

        public MappableSource(int id)
        {
            this.id = id;
        }
    }
}
