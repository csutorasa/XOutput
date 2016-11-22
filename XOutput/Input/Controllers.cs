using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input
{
    public sealed class Controllers
    {
        private List<int> ids = new List<int>();
        private object lockObject = new object();
        public int GetId()
        {
            lock(lockObject)
            {
                for(int i = 1; i <= int.MaxValue; i++)
                {
                    if(!ids.Contains(i))
                    {
                        ids.Add(i);
                        return i;
                    }
                }
                return 0;
            }
        }
        public void DisposeId(int id)
        {
            lock (lockObject)
            {
                ids.Remove(id);
            }
        }
    }
}
