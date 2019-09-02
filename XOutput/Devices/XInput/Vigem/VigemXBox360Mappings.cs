using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace XOutput.Devices.XInput.Vigem
{
    /// <summary>
    /// Mapping value helper for Vigem buttons
    /// </summary>
    public class VigemXbox360ButtonMapping
    {
        public Xbox360Button Type { get; set; }

        public VigemXbox360ButtonMapping(Xbox360Button button)
        {
            Type = button;
        }

        public bool GetValue(double value)
        {
            return value > 0.5;
        }
    }

    /// <summary>
    /// Mapping value helper for Vigem axes
    /// </summary>
    public class VigemXbox360AxisMapping
    {
        public Xbox360Axis Type { get; set; }

        public VigemXbox360AxisMapping(Xbox360Axis axis)
        {
            Type = axis;
        }

        public short GetValue(double value)
        {
            return (short)((value - 0.5) * 2 * short.MaxValue);
        }
    }

    /// <summary>
    /// Mapping value helper for Vigem axes
    /// </summary>
    public class VigemXbox360SliderMapping
    {
        public Xbox360Slider Type { get; set; }

        public VigemXbox360SliderMapping(Xbox360Slider slider)
        {
            Type = slider;
        }

        public byte GetValue(double value)
        {
            return (byte)(value * byte.MaxValue);
        }
    }
}
