using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input;

namespace XOutput.UI.Component
{
    public interface IUpdatableView
    {
        /// <summary>
        /// Updates the view from a device.
        /// </summary>
        /// <param name="device">device to update from</param>
        void updateValues(IDevice device);
    }
}
