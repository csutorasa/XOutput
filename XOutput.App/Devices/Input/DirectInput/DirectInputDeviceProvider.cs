using NLog;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using XOutput.DependencyInjection;
using XOutput.Notifications;

namespace XOutput.App.Devices.Input.DirectInput
{
    public sealed class DirectInputDeviceProvider : IInputDeviceProvider
    {
        private const string EmulatedSCPID = "028e045e-0000-0000-0000-504944564944";

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public event DeviceConnectedHandler Connected;
        public event DeviceDisconnectedHandler Disconnected;
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (value != enabled)
                {
                    enabled = value;
                    if (enabled)
                    {
                        Start();
                    }
                    else
                    {
                        Stop();
                    }
                }
            }
        }

        private readonly IgnoredDeviceService ignoredDeviceService;
        private readonly InputConfigManager inputConfigManager;
        private readonly IdHelper idHelper;
        private readonly NotificationService notificationService;
        private readonly SharpDX.DirectInput.DirectInput directInput = new SharpDX.DirectInput.DirectInput();
        private readonly List<IInputDevice> currentDevices = new List<IInputDevice>();
        private readonly object lockObject = new object();
        private bool enabled = false;
        private bool disposed = false;
        private bool allDevices = false;

        [ResolverMethod]
        public DirectInputDeviceProvider(IgnoredDeviceService ignoredDeviceService, InputConfigManager inputConfigManager, IdHelper idHelper, NotificationService notificationService)
        {
            this.ignoredDeviceService = ignoredDeviceService;
            this.inputConfigManager = inputConfigManager;
            this.idHelper = idHelper;
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
                List<string> uniqueIds = new List<string>();
                foreach (var instance in instances)
                {
                    string instanceGuid = instance.InstanceGuid.ToString();
                    string productGuid = instance.ProductGuid.ToString();
                    if (productGuid != EmulatedSCPID)
                    {
                        var device = CreateDevice(instance, uniqueIds);
                        if (device == null)
                        {
                            continue;
                        }
                        var config = inputConfigManager.LoadConfig(device);
                        device.InputConfiguration = config;
                        if (config.Autostart) {
                            device.Start();
                        }
                        currentDevices.Add(device);
                        Connected?.Invoke(this, new DeviceConnectedEventArgs(device));
                    }
                }
                foreach (var device in currentDevices.ToArray())
                {
                    if (!uniqueIds.Any(i => i == device.UniqueId))
                    {
                        currentDevices.Remove(device);
                        device.Dispose();
                        Disconnected?.Invoke(this, new DeviceDisconnectedEventArgs(device));
                    }
                }
            }
        }

        [SecurityCritical]
        private IInputDevice CreateDevice(DeviceInstance deviceInstance, List<string> uniqueIds)
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
                bool isHid = deviceInstance.IsHumanInterfaceDevice;
                string interfacePath = null;
                string uniqueIdBase;
                string hardwareId = null;
                if (isHid)
                {
                    if (ignoredDeviceService.IsIgnored(joystick.Properties.InterfacePath))
                    {
                        joystick.Dispose();
                        return null;
                    }
                    interfacePath = joystick.Properties.InterfacePath;
                    uniqueIdBase = interfacePath;
                    hardwareId = idHelper.GetHardwareId(interfacePath);
                } else {
                    uniqueIdBase = string.Join(":", deviceInstance.ProductGuid.ToString(), deviceInstance.InstanceGuid.ToString());
                }
                string uniqueId = IdHelper.GetUniqueId(uniqueIdBase);
                if (uniqueIds.Any(uid => uid == uniqueId))
                {
                    notificationService.Add(Notifications.Notifications.DirectInputInstanceIdDuplication, new[] { uniqueId }, NotificationTypes.Warning);
                }
                uniqueIds.Add(uniqueId);
                if (currentDevices.Any(d => d.UniqueId == uniqueId)) {
                    joystick.Dispose();
                    return null;
                }
                joystick.Properties.BufferSize = 128;
                return new DirectInputDevice(inputConfigManager, joystick, deviceInstance.InstanceGuid.ToString(), deviceInstance.ProductName,
                    deviceInstance.ForceFeedbackDriverGuid != Guid.Empty, uniqueId, hardwareId, interfacePath);
            }
            catch (Exception ex)
            {
                logger.Error("Failed to create device " + deviceInstance.InstanceGuid + " " + deviceInstance.InstanceName + ex.ToString());
            }
            return null;
        }

        private string GetUniqueId(DeviceInstance deviceInstance, Joystick joystick)
        {
            if (deviceInstance.IsHumanInterfaceDevice)
            {
                return IdHelper.GetUniqueId(joystick.Properties.InterfacePath);
            }
            else
            {
                return deviceInstance.InstanceGuid.ToString();
            }
        }

        public IEnumerable<IInputDevice> GetActiveDevices()
        {
            lock (lockObject)
            {
                return currentDevices;
            }
        }

        private void Start()
        {

        }

        private void Stop()
        {

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
