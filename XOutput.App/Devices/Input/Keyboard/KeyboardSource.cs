namespace XOutput.App.Devices.Input.Keyboard
{
    public class KeyboardSource : InputSource
    {

        public KeyboardSource(IInputDevice inputDevice, string name, int offset) : base(inputDevice, name, SourceTypes.Button, offset)
        {

        }


        internal bool Refresh(bool pressed)
        {
            return RefreshValue(pressed ? 1 : 0);
        }
    }
}
