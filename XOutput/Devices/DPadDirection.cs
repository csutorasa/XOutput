using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices
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
        /// <summary>
        /// Gets all <code>DPadDirection</code> values.
        /// </summary>
        public static DPadDirection[] Values => values;

        private static DPadDirection[] values = ((DPadDirection[])Enum.GetValues(typeof(DPadDirection))).Where(d => d != DPadDirection.None).ToArray();

        /// <summary>
        /// Converts 4 bool values to DPadDirection.
        /// </summary>
        /// <param name="up">Up value</param>
        /// <param name="down">Down value</param>
        /// <param name="left">Left value</param>
        /// <param name="right">Right value</param>
        /// <returns>constructed DPadDirection</returns>
        public static DPadDirection GetDirection(bool up, bool down, bool left, bool right)
        {
            DPadDirection value = DPadDirection.None;
            if (up)
            {
                value |= DPadDirection.Up;
            }
            else if (down)
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
