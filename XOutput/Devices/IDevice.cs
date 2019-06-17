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
        IEnumerable<InputSource> Sources { get; }
        /// <summary>
        /// Gets the current state of the <paramref name="inputType"/>.
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        double Get(InputSource source);
        /// <summary>
        /// Refreshes the current state. Triggers <see cref="InputChanged"/> event.
        /// </summary>
        /// <returns>if the input was available</returns>
        bool RefreshInput(bool force = false);
    }
}
