using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices
{
    /// <summary>
    /// Threadsafe method to get ID of the controllers.
    /// </summary>
    public sealed class Controllers
    {
        private List<int> ids = new List<int>();
        private object lockObject = new object();

        /// <summary>
        /// Gets a new ID.
        /// </summary>
        /// <returns></returns>
        public int GetId()
        {
            lock (lockObject)
            {
                for (int i = 1; i <= int.MaxValue; i++)
                {
                    if (!ids.Contains(i))
                    {
                        ids.Add(i);
                        return i;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// Disposes a used ID.
        /// </summary>
        /// <param name="id">ID to remove</param>
        public void DisposeId(int id)
        {
            lock (lockObject)
            {
                ids.Remove(id);
            }
        }
    }
}
