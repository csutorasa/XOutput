using System.Collections.Generic;

namespace XOutput.Mapping.Input
{
    public delegate void InputDeviceInputChanged(object sender, InputDeviceInputChangedEventArgs args);

    public class InputDeviceInputChangedEventArgs
    {
        public ISet<InputDeviceSourceWithValue> ChangedSources => changedSources;
        private readonly ISet<InputDeviceSourceWithValue> changedSources;

        public InputDeviceInputChangedEventArgs(ISet<InputDeviceSourceWithValue> changedSources)
        {
            this.changedSources = changedSources;
        }
    }
}
