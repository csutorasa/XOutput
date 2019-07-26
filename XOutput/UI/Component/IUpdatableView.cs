using XOutput.Devices;

namespace XOutput.UI.Component
{
    public interface IUpdatableView
    {
        /// <summary>
        /// Updates the view from a device.
        /// </summary>
        /// <param name="device">device to update from</param>
        void UpdateValues(IDevice device);
    }
}
