using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices
{
    /// <summary>
    /// Event delegate for SourceChanged event.
    /// </summary>
    /// <param name="sender">the disconnected <see cref="IInputDevice"/></param>
    /// <param name="e">event arguments</param>
    public delegate void SourceChangedEventHandler(object sender, SourceChangedEventArgs e);

    /// <summary>
    /// Event argument class for SourceChanged event.
    /// </summary>
    public class SourceChangedEventArgs : EventArgs
    {

    }
}
