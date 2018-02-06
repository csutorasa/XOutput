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

    public static class DPadHelper
    {
        private static DPadDirection[] values = ((DPadDirection[])Enum.GetValues(typeof(DPadDirection))).Where(d => d != DPadDirection.None).ToArray();

        public static DPadDirection[] GetValues()
        {
            return values;
        }

        public static DPadDirection GetDirection(bool up, bool down, bool left, bool right)
        {
            DPadDirection value = DPadDirection.None;
            if(up)
            {
                value |= DPadDirection.Up;
            }
            else if(down)
            {
                value |= DPadDirection.Down;
            }

            if (right)
            {
                value |= DPadDirection.Right;
            }
            else if (left)
            {
                value |= DPadDirection.Left;
            }

            return value;
        }
    }
}
