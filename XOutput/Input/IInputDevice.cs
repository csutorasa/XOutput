using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input
{
    public interface IInputDevice : IDevice, IDisposable
    {
        /// <summary>
        /// The friendly display name of the controller.
        /// </summary>
        string DisplayName { get; }
        /// <summary>
        /// Gets if the device is connected.
        /// </summary>
        /// <returns></returns>
        bool Connected { get; }
        /// <summary>
        /// If the controller has DPad
        /// </summary>
        bool HasDPad { get; }

        bool IsExclusive { get; set; }
    }
}
