using NLog;
using System.Linq;
using XOutput.Mapping.Input;
using XOutput.Message.Mappable;

namespace XOutput.Websocket.Mappable
{
    class MappableDeviceMessageHandler : IMessageHandler
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly MappableDevices mappableDevices;
        private readonly SenderFunction<MappableDeviceFeedbackResponse> senderFunction;
        private MappableDevice device;

        public MappableDeviceMessageHandler(MappableDevices mappableDevices, SenderFunction<MappableDeviceFeedbackResponse> senderFunction)
        {
            this.mappableDevices = mappableDevices;
            this.senderFunction = senderFunction;
        }

        public bool CanHandle(MessageBase message)
        {
            return message.Type == MappableDeviceDetailsRequest.MessageType || message.Type == MappableDeviceInputRequest.MessageType;
        }

        public void Handle(MessageBase message)
        {
            if (message is MappableDeviceDetailsRequest)
            {
                var detailsMessage = message as MappableDeviceDetailsRequest;
                device = mappableDevices.Create(detailsMessage.Id, detailsMessage.Name, detailsMessage.Sources.Select(s => new MappableSource(s.Id)).ToList());
                device.FeedbackReceived += DeviceFeedbackReceived;
            }
            if (message is MappableDeviceInputRequest)
            {
                if (device == null)
                {
                    logger.Warn($"Input was received to mappable device without sending details first");
                } 
                else
                {
                    var inputMessage = message as MappableDeviceInputRequest;
                    device.SetData(inputMessage.Inputs.ToDictionary(i => i.Id, i => i.Value));
                }
            }
        }

        private void DeviceFeedbackReceived(object sender, MappableDeviceFeedbackEventArgs args)
        {
            senderFunction(new MappableDeviceFeedbackResponse
            {
                SmallForceFeedback = args.SmallMotor,
                BigForceFeedback = args.BigMotor,
            });
        }

        public void Close()
        {
            if (device != null)
            {
                device.FeedbackReceived -= DeviceFeedbackReceived;
                mappableDevices.Remove(device);
            }
        }
    }
}
