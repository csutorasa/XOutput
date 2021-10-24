using NLog;
using System;
using System.Linq;
using XOutput.Mapping.Controller;
using XOutput.Mapping.Input;
using XOutput.Threading;

namespace XOutput.Websocket.Emulated
{
    class EmulatedControllerFeedbackHandler : IMessageHandler
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        
        private readonly SenderFunction<EmulatedControllerInputResponse> senderFunction;
        private IEmulatedController emulatedController;
        private ThreadContext threadContext;

        public EmulatedControllerFeedbackHandler(IEmulatedController emulatedController, SenderFunction<EmulatedControllerInputResponse> senderFunction)
        {
            this.emulatedController = emulatedController;
            this.senderFunction = senderFunction;
            threadContext = ThreadCreator.CreateLoop($"{emulatedController.Id} input device report thread", SendFeedback, 20);
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
            senderFunction(new EmulatedControllerInputResponse
            {
                Sources = emulatedController.GetSources().Select(s => new EmulatedControllerSourceValue {
                    Id = s.Key,
                    Value = s.Value,
                }).ToList(),
                Targets = emulatedController.GetTargets().Select(t => new EmulatedControllerTargetValue {
                    Id = t.Key,
                    Value = t.Value,
                }).ToList(),
            });
        }

        public void Close()
        {
            threadContext.Cancel();
        }
    }
}
