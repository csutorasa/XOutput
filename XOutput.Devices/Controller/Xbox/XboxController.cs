namespace XOutput.Devices.Controller.Xbox
{
    public abstract class XboxController : ControllerBase<XboxInputTypes>
    {
        protected override double GetDefaultValue(XboxInputTypes input)
        {
            return input.GetDefaultValue();
        }
    }
}
