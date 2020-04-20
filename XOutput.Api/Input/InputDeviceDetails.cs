
using System.Collections.Generic;

namespace XOutput.Api.Input
{
    public class InputDeviceDetails
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string HardwareId { get; set; }
        public List<InputDeviceSource> Sources { get; set; }
        public List<InputForceFeedback> ForceFeedbacks { get; set; }
    }

    public class InputDeviceSource
    {
        public int Offset { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class InputForceFeedback
    {
        public int Offset { get; set; }
    }
}
