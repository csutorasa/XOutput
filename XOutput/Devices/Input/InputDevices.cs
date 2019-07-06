using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices.Input
{
    public class InputDevices
    {

        private static InputDevices instance = new InputDevices();
        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        public static InputDevices Instance => instance;

        private List<IInputDevice> inputDevices = new List<IInputDevice>();

        protected InputDevices()
        {

        }

        public void Add(IInputDevice inputDevice)
        {
            inputDevices.Add(inputDevice);
            Controllers.Instance.Update(inputDevices);
        }

        public void Remove(IInputDevice inputDevice)
        {
            inputDevices.Remove(inputDevice);
        }

        public IEnumerable<IInputDevice> GetDevices()
        {
            return inputDevices.ToArray();
        }
    }
}
