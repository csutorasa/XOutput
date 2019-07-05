using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices.Input
{
    /// <summary>
    /// Main interface of input devices.
    /// </summary>
    public interface IInputDevice : IDevice, IDisposable
    {
        /// <summary>
        /// This event is invoked if the device is disconnected.
        /// </summary>
        event DeviceDisconnectedHandler Disconnected;
        /// <summary>
        /// The friendly display name of the controller.
        /// </summary>
        string DisplayName { get; }
        /// <summary>
        /// The unique ID of the controller.
        /// </summary>
        string UniqueId { get; }
        /// <summary>
        /// Gets if the device is connected.
        /// </summary>
        bool Connected { get; }
        /// <summary>
        /// Gets the hardware ID of the device.
        /// </summary>
        string HardwareID { get; }
        /// <summary>
        /// Gets the number of force feedback motors.
        /// </summary>
        int ForceFeedbackCount { get; }
        /// <summary>
        /// Gets input configuration.
        /// </summary>
        InputConfig InputConfiguration { get; }
        /// <summary>
        /// Sets the force feedback motor values.
        /// </summary>
        /// <param name="big">Big motor value</param>
        /// <param name="small">Small motor value</param>
        void SetForceFeedback(double big, double small);
    }
}
