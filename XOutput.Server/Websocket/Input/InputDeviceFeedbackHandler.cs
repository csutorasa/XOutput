using System.Linq;
using XOutput.Mapping.Input;
using XOutput.Threading;

namespace XOutput.Websocket.Input
{
    class InputDeviceFeedbackHandler : MessageHandler
    {
        private InputDevice device;
        private ThreadContext threadContext;

        public InputDeviceFeedbackHandler(CloseFunction closeFunction, SenderFunction senderFunction, InputDevice inputDevice) : base(closeFunction, senderFunction)
        {
            this.device = inputDevice;
            threadContext = ThreadCreator.CreateLoop($"{device.Id} input device report thread", SendFeedback, 20);
            threadContext.Start();
        }

        private void SendFeedback()
        {
            senderFunction(new InputDeviceInputResponse
            {
                Sources = device.FindAllSources().Select(s => new InputDeviceSourceValue {
                    Id = s.Id,
                    Value = s.Value,
                }).ToList(),
                Targets = device.FindAllTargets().Select(s => new InputDeviceTargetValue {
                    Id = s.Id,
                    Value = s.Value,
                }).ToList(),
            });
        }

        public override void Close()
        {
            base.Close();
            threadContext.Cancel();
        }
    }
}
