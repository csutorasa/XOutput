using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input
{
    /// <summary>
    /// Main interface of input devices.
    /// </summary>
    public interface IInputDevice : IDevice, IDisposable
    {
        /// <summary>
        /// This event is invoked if the device is disconnected
        /// </summary>
        event Action Disconnected;
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
    }
}
