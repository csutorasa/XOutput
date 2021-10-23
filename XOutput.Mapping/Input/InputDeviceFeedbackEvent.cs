using System.Collections.Generic;

namespace XOutput.Mapping.Input
{
    public delegate void InputDeviceFeedback(object sender, InputDeviceFeedbackEventArgs args);

    public class InputDeviceFeedbackEventArgs
    {
        public IEnumerable<InputDeviceTargetWithValue> Targets { get; private set; }
        
        public InputDeviceFeedbackEventArgs(IEnumerable<InputDeviceTargetWithValue> targets)
        {
            Targets = targets;
        }
    }
}
