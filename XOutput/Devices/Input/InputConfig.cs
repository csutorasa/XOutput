namespace XOutput.Devices.Input
{
    public class InputConfig
    {
        /// <summary>
        /// Enables the force feedback for the controller.
        /// </summary>
        public bool ForceFeedback { get; set; }

        public InputConfig()
        {
            ForceFeedback = false;
        }

        public InputConfig(int forceFeedbackCount)
        {
            ForceFeedback = forceFeedbackCount > 0;
        }
    }
}
