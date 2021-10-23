using XOutput.Common.Devices;

namespace XOutput.App.Devices.Input.Mouse
{
    public class MouseSource : InputSource
    {

        public MouseSource(IInputDevice inputDevice, string name, int offset) : base(inputDevice, name, SourceTypes.Button, offset)
        {

        }


        internal bool Refresh(bool pressed)
        {
            return RefreshValue(pressed ? 1 : 0);
        }
    }
}
