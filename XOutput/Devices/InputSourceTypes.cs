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
        AxisX = 4,
        AxisY = 8,
        AxisZ = 16,
        Axis = AxisX | AxisY | AxisZ,
    }
}
