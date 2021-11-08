using NLog;
using System;
using System.Linq;
using XOutput.Mapping.Controller;
using XOutput.Threading;

namespace XOutput.Websocket.Emulation
{
    class EmulatedControllerFeedbackHandler : IMessageHandler
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        
        private readonly SenderFunction<ControllerInputResponse> senderFunction;
        private IMappedController emulatedController;
        private ThreadContext threadContext;

        public EmulatedControllerFeedbackHandler(IMappedController emulatedController, SenderFunction<ControllerInputResponse> senderFunction)
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
            senderFunction(new ControllerInputResponse
            {
                Sources = emulatedController.GetSources().Select(s => new ControllerSourceValue {
                    Id = s.Key,
                    Value = s.Value,
                }).ToList(),
                Targets = emulatedController.GetTargets().Select(t => new ControllerTargetValue {
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
