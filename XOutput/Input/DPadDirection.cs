using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput
{
    /// <summary>
    /// Contains the possible values for DPad
    /// </summary>
    [Flags]
    public enum DPadDirection
    {
        None = 0,
        Up = 1 << 0,
        Left = 1 << 1,
        Down = 1 << 2,
        Right = 1 << 3
    }
}
