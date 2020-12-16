namespace XOutput.Mapping.Input
{
    public delegate void MappableDeviceFeedback(object sender, MappableDeviceFeedbackEventArgs args);

    public class MappableDeviceFeedbackEventArgs
    {
        public double SmallMotor { get; private set; }
        public double BigMotor { get; private set; }

        public MappableDeviceFeedbackEventArgs(double smallMotor, double bigMotor)
        {
            SmallMotor = smallMotor;
            BigMotor = bigMotor;
        }
    }
}
