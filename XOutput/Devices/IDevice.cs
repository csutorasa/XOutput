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
        /// Gets the current state of the DPads.
        /// </summary>
        IEnumerable<DPadDirection> DPads { get; }
        /// <summary>
        /// Gets all Enum values that represent button.
        /// </summary>
        IEnumerable<Enum> Buttons { get; }
        /// <summary>
        /// Gets all Enum values that represent axis.
        /// </summary>
        IEnumerable<Enum> Axes { get; }
        /// <summary>
        /// Gets all Enum values that represent slider.
        /// </summary>
        IEnumerable<Enum> Sliders { get; }
        /// <summary>
        /// Gets the current state of the <paramref name="inputType"/>.
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        double Get(Enum inputType);
    }
}
