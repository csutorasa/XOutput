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
        /// The identification string of the device.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Gets if the device is connected.
        /// </summary>
        bool Connected { get; }
        /// <summary>
        /// Gets the number of force feedback motors.
        /// </summary>
        int ForceFeedbackCount { get; }
        /// <summary>
        /// Sets the force feedback motor values.
        /// </summary>
        /// <param name="big">Big motor value</param>
        /// <param name="small">Small motor value</param>
        void SetForceFeedback(double big, double small);
        /// <summary>
        /// Gets the current raw state of the <paramref name="inputType"/>.
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        double GetRaw(Enum inputType);
    }
}
