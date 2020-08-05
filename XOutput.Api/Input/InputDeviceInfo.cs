
using System.Collections.Generic;

namespace XOutput.Api.Input
{
    public class InputDeviceInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> ActiveFeatures { get; set; }
    }
}
