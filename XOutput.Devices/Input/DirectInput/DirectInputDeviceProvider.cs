using NLog;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Notifications;

namespace XOutput.Devices.Input.DirectInput
{
    public sealed class DirectInputDeviceProvider : IInputDeviceProvider
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public event DeviceConnectedHandler Connected;
        public event DeviceDisconnectedHandler Disconnected;

        private readonly IgnoredDeviceService ignoredDeviceService;
        private readonly InputConfigManager inputConfigManager;
        private readonly NotificationService notificationService;
        private readonly SharpDX.DirectInput.DirectInput directInput = new SharpDX.DirectInput.DirectInput();
        private readonly List<IInputDevice> currentDevices = new List<IInputDevice>();
        private readonly object lockObject = new object();
        private bool disposed = false;
        bool allDevices = false;

        [ResolverMethod]
        public DirectInputDeviceProvider(IgnoredDeviceService ignoredDeviceService, InputConfigManager inputConfigManager, NotificationService notificationService)
        {
            this.ignoredDeviceService = ignoredDeviceService;
            this.inputConfigManager = inputConfigManager;
            this.notificationService = notificationService;
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
                    string instanceGuid = instance.InstanceGuid.ToString();
                    string productGuid = instance.ProductGuid.ToString();
                    if (!ignoredDeviceService.IsIgnored(productGuid, instanceGuid) && !currentDevices.Any(d => d.UniqueId == instanceGuid))
                    {
                        var device = CreateDevice(instance);
                        if (device == null)
                        {
                            continue;
                        }
                        if (currentDevices.Any(d => d.UniqueId == device.UniqueId))
                        {
                            notificationService.Add(Notifications.DirectInputInstanceIdDuplication, new[] { device.UniqueId }, NotificationTypes.Warning);
                        }
                        var config = inputConfigManager.LoadConfig(device.UniqueId);
                        device.InputConfiguration = config;
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

        [HandleProcessCorruptedStateExceptions]
        private IInputDevice CreateDevice(DeviceInstance deviceInstance)
        {
            try
            {
                if (!directInput.IsDeviceAttached(deviceInstance.InstanceGuid))
                {
                    return null;
                }
                var joystick = new Joystick(directInput, deviceInstance.InstanceGuid);
                if (joystick.Capabilities.AxeCount < 1 && joystick.Capabilities.ButtonCount < 1)
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
    }
}
