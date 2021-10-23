using System;

namespace XOutput.Common.Devices
{
    [Flags]
    public enum SourceTypes
    {
        None = 0,
        Button = 1,
        Slider = 2,
        Dpad = 4,
        AxisX = 8,
        AxisY = 16,
        AxisZ = 32,
        Axis = AxisX | AxisY | AxisZ,
    }

    public static class SourceTypesExtension
    {
        public static bool IsAxis(this SourceTypes type)
        {
            return SourceTypes.Axis.HasFlag(type) && type != SourceTypes.None;
        }
    }
}
