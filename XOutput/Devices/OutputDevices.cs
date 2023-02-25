using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Devices.Input;
using XOutput.Devices.XInput;
using XOutput.Devices.XInput.SCPToolkit;
using XOutput.Devices.XInput.Vigem;
using XOutput.Logging;
using XOutput.UI.Windows;

namespace XOutput.Devices
{
    public class OutputDevices
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(OutputDevices));
        private static OutputDevices instance = new OutputDevices();
        
        public static OutputDevices Instance => instance;
        
        private readonly List<int> ids = new List<int>();
        private readonly object lockObject = new object();
        private readonly List<IXOutputInterface> outputDevices = new List<IXOutputInterface>();
        public const int MaxOutputDevices = 4;
        
        private OutputDevices()
        {
            InitializeDevices();
        }

        private void InitializeDevices()
        {
            if (outputDevices.Any())
            {
                return;
            }
            for (var i = 0; i < MaxOutputDevices; i++)
            {
                var device = CreateDevice();
                device.Plugin(i);
                outputDevices.Add(device);
            }
        }

        private IXOutputInterface CreateDevice()
        {
            if (VigemDevice.IsAvailable())
            {
                logger.Info("ViGEm devices are used.");
                return new VigemDevice();
            }
            else if (ScpDevice.IsAvailable())
            {
                logger.Warning("SCP Toolkit devices are used.");
                return new ScpDevice();
            }
            else
            {
                logger.Error("Neither ViGEm nor SCP devices can be used.");
                return null;
            }
        }
        
        public void Add(IXOutputInterface xOutputInterface)
        {
            outputDevices.Add(xOutputInterface);
        }

        public List<IXOutputInterface> GetDevices()
        {
            return outputDevices;
        }
    }
}