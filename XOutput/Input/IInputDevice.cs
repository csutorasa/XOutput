using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input
{
    public interface IInputDevice : IDisposable
    {
        /// <summary>
        /// This event is invoked if the data from the device was updated
        /// </summary>
        event Action InputChanged;
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
        /// <summary>
        /// If the controller has axes
        /// </summary>
        bool HasAxes { get; }
        /// <summary>
        /// If the controller has buttons
        /// </summary>
        int ButtonCount { get; }
        /// <summary>
        /// Gets the current state of the DPad.
        /// </summary>
        /// <returns></returns>
        DPadDirection DPad { get; }
        /// <summary>
        /// Gets the current state of the inputTpye.
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        double Get(Enum inputType);
        IEnumerable<Enum> GetButtons();
        IEnumerable<Enum> GetAxes();
    }
}
