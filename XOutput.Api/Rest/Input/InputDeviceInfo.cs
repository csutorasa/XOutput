
using System.Collections.Generic;
using XOutput.Common.Input;

namespace XOutput.Rest.Input
{
    public class InputDeviceInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DeviceApi { get; set; }
        public IEnumerable<InputDeviceSource> Sources { get; set; }
        public IEnumerable<InputDeviceTarget> Targets { get; set; }
    }
}
