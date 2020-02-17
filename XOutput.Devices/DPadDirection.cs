using System;
using System.Linq;

namespace XOutput.Devices
{
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
        public static DPadDirection[] Values => values;

        private static DPadDirection[] values = Enum.GetValues(typeof(DPadDirection)).OfType<DPadDirection>().Where(d => d != DPadDirection.None).ToArray();

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
