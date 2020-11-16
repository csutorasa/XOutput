using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Api.Message;
using XOutput.Api.Message.Input;
using XOutput.Core.Threading;
using XOutput.Devices.Input;

namespace XOutput.Server.Websocket.Input
{
    class InputValuesMessageHandler : IMessageHandler
    {
        private readonly InputDeviceHolder device;
        private readonly SenderFunction<InputValuesMessage> senderFunction;
        private readonly ThreadContext threadContext;
        private readonly object lockObject = new object();
        private readonly ISet<InputValueData> changedValues = new HashSet<InputValueData>();

        public InputValuesMessageHandler(InputDeviceHolder device, SenderFunction<InputValuesMessage> senderFunction)
        {
            this.device = device;
            this.senderFunction = senderFunction;

            var devices = device.GetInputDevices();
            device.Connected += InputConnected;
            device.Disconnected += InputDisconnected;
            foreach(var inputDevice in devices) {
                inputDevice.InputChanged += InputChanged;
            }
            threadContext = ThreadCreator.CreateLoop($"Websocket input value writer {device.DisplayName}", ResponseLoop, 33).Start();
        }

        private void InputConnected(object sender, DeviceConnectedEventArgs e)
        {
            e.Device.InputChanged += InputChanged;
        }

        private void InputDisconnected(object sender, DeviceDisconnectedEventArgs e)
        {
            e.Device.InputChanged += InputChanged;
        }

        private void ResponseLoop()
        {
            lock (lockObject)
            {
                if (changedValues.Count > 0)
                {
                    senderFunction?.Invoke(new InputValuesMessage
                    {
                        Values = changedValues.ToList(),
                    });
                    changedValues.Clear();
                }
            }
        }

        private void InputChanged(object sender, DeviceInputChangedEventArgs e)
        {
            lock (lockObject)
            {
                foreach (var source in e.ChangedValues)
                {
                    changedValues.Add(new InputValueData {
                        Offset = source.Offset,
                        Method = e.Device.InputMethod.ToString(),
                        Value = source.GetValue(),
                    });
                }
            }
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
            foreach(var inputDevice in device.GetInputDevices()) {
                inputDevice.InputChanged -= InputChanged;
            }
            threadContext.Cancel().Wait();
        }
    }
}
