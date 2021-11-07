using NLog;
using System;
using System.Linq;
using XOutput.Mapping.Input;
using XOutput.Threading;

namespace XOutput.Websocket.Input
{
    class InputDeviceFeedbackHandler : IMessageHandler
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly SenderFunction<InputDeviceInputResponse> senderFunction;
        private InputDevice device;
        private ThreadContext threadContext;

        public InputDeviceFeedbackHandler(InputDevice inputDevice, SenderFunction<InputDeviceInputResponse> senderFunction)
        {
            this.device = inputDevice;
            this.senderFunction = senderFunction;
            threadContext = ThreadCreator.CreateLoop($"{device.Id} input device report thread", SendFeedback, 20);
            threadContext.Start();
        }

        public bool CanHandle(MessageBase message)
        {
            return false;
        }

        public void Handle(MessageBase message)
        {
            throw new NotImplementedException();
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

        public void Close()
        {
            threadContext.Cancel();
        }
    }
}
