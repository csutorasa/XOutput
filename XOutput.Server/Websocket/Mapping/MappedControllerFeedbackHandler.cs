using System.Linq;
using XOutput.Mapping.Controller;
using XOutput.Threading;
using XOutput.Websocket.Emulation;

namespace XOutput.Websocket.Mapping
{
    class MappedControllerFeedbackHandler : MessageHandler
    {        private IMappedController emulatedController;
        private ThreadContext threadContext;

        public MappedControllerFeedbackHandler(CloseFunction closeFunction, SenderFunction senderFunction, IMappedController emulatedController) : base(closeFunction, senderFunction)
        {
            this.emulatedController = emulatedController;
            threadContext = ThreadCreator.CreateLoop($"{emulatedController.Id} input device report thread", SendFeedback, 20);
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

        public override void Close()
        {
            base.Close();
            threadContext.Cancel();
        }
    }
}
