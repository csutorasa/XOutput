using NLog;
using System.Linq;
using XOutput.Api.Message;
using XOutput.Mapping.Input;
using XOutput.Message.Mappable;

namespace XOutput.Server.Websocket.Mappable
{
    class MappableDeviceMessageHandler : IMessageHandler
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly MappableDevices mappableDevices;
        private readonly SenderFunction<MappableDeviceFeedbackMessage> senderFunction;
        private MappableDevice device;

        public MappableDeviceMessageHandler(MappableDevices mappableDevices, SenderFunction<MappableDeviceFeedbackMessage> senderFunction)
        {
            this.mappableDevices = mappableDevices;
            this.senderFunction = senderFunction;
        }

        public bool CanHandle(MessageBase message)
        {
            return message.Type == MappableDeviceDetailsMessage.MessageType || message.Type == MappableDeviceInputMessage.MessageType;
        }

        public void Handle(MessageBase message)
        {
            if (message is MappableDeviceDetailsMessage)
            {
                var detailsMessage = message as MappableDeviceDetailsMessage;
                device = mappableDevices.Create(detailsMessage.Id, detailsMessage.Name, detailsMessage.Sources.Select(s => new MappableSource(s.Id)).ToList());
                device.FeedbackReceived += DeviceFeedbackReceived;
            }
            if (message is MappableDeviceInputMessage)
            {
                if (device == null)
                {
                    logger.Warn($"Input was received to mappable device without sending details first");
                } 
                else
                {
                    var inputMessage = message as MappableDeviceInputMessage;
                    device.SetData(inputMessage.Inputs.ToDictionary(i => i.Id, i => i.Value));
                }
            }
        }

        private void DeviceFeedbackReceived(object sender, MappableDeviceFeedbackEventArgs args)
        {
            senderFunction(new MappableDeviceFeedbackMessage
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
