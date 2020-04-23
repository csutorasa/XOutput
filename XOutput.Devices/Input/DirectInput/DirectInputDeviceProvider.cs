using NLog;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;

namespace XOutput.Devices.Input.DirectInput
{
    public sealed class DirectInputDeviceProvider : InputConfigManager, IInputDeviceProvider
    {
        private const string EmulatedSCPID = "028e045e-0000-0000-0000-504944564944";
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public event DeviceConnectedHandler Connected;
        public event DeviceDisconnectedHandler Disconnected;

        private readonly SharpDX.DirectInput.DirectInput directInput = new SharpDX.DirectInput.DirectInput();
        private readonly List<IInputDevice> currentDevices = new List<IInputDevice>();
        private readonly object lockObject = new object();
        private bool disposed = false;
        bool allDevices = false;

        [ResolverMethod]
        public DirectInputDeviceProvider(ConfigurationManager configurationManager) : base(configurationManager)
        {

        }

        public void SearchDevices()
        {
            lock (lockObject)
            {
                IEnumerable<DeviceInstance> instances = directInput.GetDevices();
                if (allDevices)
                {
                    instances = instances.Where(di => di.Type != DeviceType.Keyboard && di.Type != DeviceType.Mouse).ToList();
                }
                else
                {
                    instances = instances.Where(di => di.Type == DeviceType.Joystick || di.Type == DeviceType.Gamepad || di.Type == DeviceType.FirstPerson).ToList();
                }
                foreach (var instance in instances)
                {
                    string guid = instance.InstanceGuid.ToString();
                    if (!currentDevices.Any(d => d.UniqueId == guid))
                    {
                        var device = CreateDevice(instance);
                        if (device == null)
                        {
                            continue;
                        }
                        var config = LoadConfig(device.UniqueId);
                        currentDevices.Add(device);
                        Connected?.Invoke(this, new DeviceConnectedEventArgs(device));
                    }
                }
                foreach (var device in currentDevices.ToArray())
                {
                    string guid = device.UniqueId;
                    if (!instances.Any(i => i.InstanceGuid.ToString() == guid))
                    {
                        currentDevices.Remove(device);
                        device.Dispose();
                        Disconnected?.Invoke(this, new DeviceDisconnectedEventArgs(device));
                    }
                }
            }
        }

        private IInputDevice CreateDevice(DeviceInstance deviceInstance)
        {
            try
            {
                var joystick = new Joystick(directInput, deviceInstance.InstanceGuid);
                if (joystick.Information.ProductGuid.ToString() == EmulatedSCPID || (joystick.Capabilities.AxeCount < 1 && joystick.Capabilities.ButtonCount < 1))
                {
                    joystick.Dispose();
                    return null;
                }
                joystick.Properties.BufferSize = 128;
                var device = new DirectInputDevice(joystick, deviceInstance.InstanceGuid.ToString(), deviceInstance.ProductName,
                    deviceInstance.ForceFeedbackDriverGuid != Guid.Empty, deviceInstance.IsHumanInterfaceDevice);
                return device;
            }
            catch (Exception ex)
            {
                logger.Error("Failed to create device " + deviceInstance.InstanceGuid + " " + deviceInstance.InstanceName + ex.ToString());
                return null;
            }
        }

        public IEnumerable<IInputDevice> GetActiveDevices()
        {
            lock (lockObject)
            {
                return currentDevices;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                currentDevices.ForEach(d => d.Dispose());
                directInput.Dispose();
            }
            disposed = true;
        }

        public bool SaveInputConfig(string id, InputConfig config)
        {
            var device = GetActiveDevices().FirstOrDefault(d => d.UniqueId == id);
            if (device == null)
            {
                return false;
            }
            SaveConfig($"conf/input/{device.UniqueId}.json", config);
            device.InputConfiguration = config;
            return true;
        }
    }
}
