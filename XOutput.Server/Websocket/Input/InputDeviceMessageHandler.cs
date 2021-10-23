using NLog;
using System;
using System.Linq;
using XOutput.Common.Input;
using XOutput.Mapping.Input;

namespace XOutput.Websocket.Input
{
    class InputDeviceMessageHandler : IMessageHandler
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly InputDevices inputDevices;
        private readonly SenderFunction<InputDeviceFeedbackResponse> senderFunction;
        private InputDevice device;

        public InputDeviceMessageHandler(InputDevices inputDevices, SenderFunction<InputDeviceFeedbackResponse> senderFunction)
        {
            this.inputDevices = inputDevices;
            this.senderFunction = senderFunction;
        }

        public bool CanHandle(MessageBase message)
        {
            return message.Type == InputDeviceDetailsRequest.MessageType || message.Type == InputDeviceInputRequest.MessageType;
        }

        public void Handle(MessageBase message)
        {
            if (message is InputDeviceDetailsRequest)
            {
                var detailsMessage = message as InputDeviceDetailsRequest;
                var deviceApi = (InputDeviceApi) Enum.Parse(typeof(InputDeviceApi), detailsMessage.InputApi);
                device = inputDevices.Create(detailsMessage.Id, detailsMessage.Name, deviceApi, detailsMessage.Sources.Select(InputDeviceSourceWithValue.Create).ToList(), detailsMessage.Targets.Select(InputDeviceTargetWithValue.Create).ToList());
                device.FeedbackReceived += DeviceFeedbackReceived;
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

        public void Close()
        {
            if (device != null)
            {
                device.FeedbackReceived -= DeviceFeedbackReceived;
                inputDevices.Remove(device);
            }
        }
    }
}
