using System;
using System.Collections.Generic;
using System.Text;

namespace XOutput.Api.Devices
{
    public class DeviceInfo
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public string DeviceType { get; set; }
        public string Emulator { get; set; }
        public bool Active { get; set; }
        public bool Local { get; set; }
    }
}
