using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices
{
    /// <summary>
    /// Main interface of devices.
    /// </summary>
    public interface IDevice : IDisposable
    {
        /// <summary>
        /// This event is invoked if the data from the device was updated.
        /// </summary>
        event DeviceInputChangedHandler InputChanged;
        /// <summary>
        /// This event is invoked if the device is disconnected.
        /// </summary>
        event DeviceDisconnectedHandler Disconnected;
        /// <summary>
        /// The friendly display name of the controller.
        /// </summary>
        string DisplayName { get; }
        /// <summary>
        /// Gets all values of the enum
        /// </summary>
        IEnumerable<InputType> Values { get; }
        /// <summary>
        /// Gets all button values of the enum
        /// </summary>
        IEnumerable<InputType> Buttons { get; }
        /// <summary>
        /// Gets all axis values of the enum
        /// </summary>
        IEnumerable<InputType> Axes { get; }
        /// <summary>
        /// Gets all DPad values of the enum
        /// </summary>
        IEnumerable<DPadDirection> DPads { get; }
        /// <summary>
        /// Gets all Slider values of the enum
        /// </summary>
        IEnumerable<InputType> Sliders { get; }
        /// <summary>
        /// Gets all Slider values of the enum
        /// </summary>
        string ConvertToString(InputType type);
        /// <summary>
        /// Gets the current state of the <paramref name="inputType"/>.
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        double Get(InputType inputType);
    }
}
