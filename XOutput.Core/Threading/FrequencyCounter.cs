using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Core.Threading
{
    public class FrequencyCounter
    {
        private readonly object lockObject = new object();
        private readonly List<DateTime> occurences = new List<DateTime>();
        private readonly TimeSpan span;

        public FrequencyCounter(TimeSpan span)
        {
            this.span = span;
        }

        public int Add()
        {
            lock (lockObject)
            {
                occurences.Add(DateTime.Now);
            }
            return GetOccurences();
        }

        public int GetOccurences()
        {
            DeleteOldOccurences();
            return occurences.Count;
        }

        private void DeleteOldOccurences()
        {
            DateTime limit = DateTime.Now.Subtract(span);
            lock (lockObject)
            {
                foreach (var occurence in occurences.ToArray())
                {
                    if (occurence < limit)
                    {
                        occurences.RemoveAt(0);
                    }
                }
            }
        }
    }
}
