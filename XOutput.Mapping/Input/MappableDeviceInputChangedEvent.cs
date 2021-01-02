using System.Collections.Generic;

namespace XOutput.Mapping.Input
{
    public delegate void MappableDeviceInputChanged(object sender, MappableDeviceInputChangedEventArgs args);

    public class MappableDeviceInputChangedEventArgs
    {
        public ISet<MappableSource> ChangedSources => changedSources;
        private readonly ISet<MappableSource> changedSources;

        public MappableDeviceInputChangedEventArgs(ISet<MappableSource> changedSources)
        {
            this.changedSources = changedSources;
        }
    }
}
