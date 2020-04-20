using System;
using System.Linq;
using XOutput.Api.Message;
using XOutput.Api.Message.Input;
using XOutput.Devices.Input;

namespace XOutput.Server.Websocket.Input
{
    class InputValuesMessageHandler : IMessageHandler
    {
        private readonly IInputDevice device;
        private readonly SenderFunction<InputValuesMessage> senderFunction;

        public InputValuesMessageHandler(IInputDevice device, SenderFunction<InputValuesMessage> senderFunction)
        {
            this.device = device;
            this.senderFunction = senderFunction;
            device.InputChanged += InputChanged;
        }

        private void InputChanged(object sender, DeviceInputChangedEventArgs e)
        {
            senderFunction?.Invoke(new InputValuesMessage
            {
                Values = e.ChangedValues.ToDictionary(v => v.Offset, v => v.GetValue())
            });
        }

        public bool CanHandle(MessageBase message)
        {
            return false;
        }

        public void Handle(MessageBase message)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            device.InputChanged -= InputChanged;
        }
    }
}
