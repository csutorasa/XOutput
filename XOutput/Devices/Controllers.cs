using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.Input;

namespace XOutput.Devices
{
    /// <summary>
    /// Threadsafe method to get ID of the controllers.
    /// </summary>
    public sealed class Controllers
    {
        private static Controllers instance = new Controllers();
        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        public static Controllers Instance => instance;

        private List<int> ids = new List<int>();
        private object lockObject = new object();
        private List<GameController> controllers = new List<GameController>();

        private Controllers()
        {

        }

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

        public void Add(GameController controller, bool refresh = false)
        {
            controllers.Add(controller);
            Update(controller, InputDevices.Instance.GetDevices());
        }

        public void Remove(GameController controller)
        {
            controllers.Remove(controller);
        }

        public void Update(GameController controller, IEnumerable<IInputDevice> inputDevices)
        {
            controller.Mapper.Attach(inputDevices);
            controller.XInput.UpdateSources(controller.Mapper.GetInputs());
        }

        public void Update(IEnumerable<IInputDevice> inputDevices)
        {
            foreach (var controller in controllers)
            {
                Update(controller, inputDevices);
            }
        }

        public IEnumerable<GameController> GetControllers()
        {
            return controllers.ToArray();
        }
    }
}
