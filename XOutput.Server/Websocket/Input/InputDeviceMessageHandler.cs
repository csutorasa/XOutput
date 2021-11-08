using NLog;
using System;
using System.Linq;
using XOutput.Common.Input;
using XOutput.Mapping.Input;

namespace XOutput.Websocket.Input
{
    class InputDeviceMessageHandler : MessageHandler
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly InputDevices inputDevices;
        private InputDevice device;

        public InputDeviceMessageHandler(CloseFunction closeFunction, SenderFunction senderFunction, InputDevices inputDevices) : base(closeFunction, senderFunction)
        {
            this.inputDevices = inputDevices;
        }

        protected override void HandleMessage(MessageBase message)
        {
            if (message is InputDeviceDetailsRequest)
            {
                if (device != null)
                {
                    logger.Warn($"Input device was received multiple times");
                } else {
                    var detailsMessage = message as InputDeviceDetailsRequest;
                    var deviceApi = (InputDeviceApi) Enum.Parse(typeof(InputDeviceApi), detailsMessage.InputApi);
                    device = inputDevices.Create(detailsMessage.Id, detailsMessage.Name, deviceApi, detailsMessage.Sources.Select(InputDeviceSourceWithValue.Create).ToList(), detailsMessage.Targets.Select(InputDeviceTargetWithValue.Create).ToList());
                    device.FeedbackReceived += DeviceFeedbackReceived;
                }
            }
            if (message is InputDeviceInputRequest)
            {
                if (device == null)
                {
                    logger.Warn($"Input was received to Input device without sending details first");
                } 
                else
                {
                    var inputMessage = message as InputDeviceInputRequest;
                    device.SetData(inputMessage.Inputs.ToDictionary(i => i.Id, i => i.Value));
                }
            }
        }

        private void DeviceFeedbackReceived(object sender, InputDeviceFeedbackEventArgs args)
        {
            senderFunction(new InputDeviceFeedbackResponse
            {
                Targets = args.Targets.Select(t => new InputDeviceTargetValue { Id = t.Id, Value = t.Value }).ToList(),
            });
        }

        public override void Close()
        {
            base.Close();
            if (device != null)
            {
                device.FeedbackReceived -= DeviceFeedbackReceived;
                inputDevices.Remove(device);
            }
        }
    }
}
