using System;
using System.Collections.Generic;
using XOutput.Core.Configuration;

namespace XOutput.Devices.Input
{
    public enum InputDeviceMethod
    {
        WindowsApi,
        DirectInput,
        RawInput,
        Websocket,
    }
}
