using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices
{
    /// <summary>
    /// Source types.
    /// </summary>
    [Flags]
    public enum InputSourceTypes
    {
        Disabled = 0,
        Button = 1,
        Slider = 2,
        Dpad = 4,
        AxisX = 8,
        AxisY = 16,
        AxisZ = 32,
        Axis = AxisX | AxisY | AxisZ,
    }
}
