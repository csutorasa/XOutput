using HidSharp;
using HidSharp.Reports;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using XOutput.DependencyInjection;

namespace XOutput.App.Devices.Input.RawInput
{
    public class RawInputDeviceProvider : IInputDeviceProvider
    {
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
        private readonly List<IInputDevice> currentDevices = new List<IInputDevice>();
        private readonly object lockObject = new object();
        private bool enabled = false;
        private bool disposed = false;
        private bool allDevices = false;

        [ResolverMethod]
        public RawInputDeviceProvider(IgnoredDeviceService ignoredDeviceService, InputConfigManager inputConfigManager, IdHelper idHelper)
        {
            this.ignoredDeviceService = ignoredDeviceService;
            this.inputConfigManager = inputConfigManager;
            this.idHelper = idHelper;
        }


        public IEnumerable<IInputDevice> GetActiveDevices()
        {
            lock (lockObject)
            {
                return currentDevices;
            }
        }

        public void SearchDevices()
        {
            var local = DeviceList.Local;
            var devices = local.GetDevices(DeviceTypes.Hid).OfType<HidDevice>();
            List<string> instances = new List<string>();
            lock (lockObject)
            {
                foreach (var device in devices)
                {
                    if (ignoredDeviceService.IsIgnored(device.DevicePath))
                    {
                        continue;
                    }
                    string uniqueId = IdHelper.GetUniqueId(device.DevicePath);
                    instances.Add(uniqueId);
                    if (currentDevices.Any(d => d.UniqueId == uniqueId))
                    {
                        continue;
                    }
                    try
                    {
                        HidStream hidStream;
                        if (device.TryOpen(out hidStream))
                        {
                            hidStream.ReadTimeout = Timeout.Infinite;
                            var reportDescriptor = device.GetReportDescriptor();
                            var deviceItems = reportDescriptor.DeviceItems.Where(i => Match(i,
                                Usage.GenericDesktopGamepad, Usage.GenericDesktopJoystick, Usage.GenericDesktopMultiaxisController));
                            foreach (var deviceItem in deviceItems)
                            {
                                var inputDevice = new RawInputDevice(inputConfigManager, idHelper, device, reportDescriptor, deviceItem, hidStream, uniqueId);
                                var config = inputConfigManager.LoadConfig(inputDevice);
                                inputDevice.InputConfiguration = config;
                                if (config.Autostart) {
                                    inputDevice.Start();
                                }
                                currentDevices.Add(inputDevice);
                                Connected?.Invoke(this, new DeviceConnectedEventArgs(inputDevice));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Warn(e, $"Ignoring {device.DevicePath} temporarily due to error");
                        ignoredDeviceService.AddTemporaryIgnore(device.DevicePath);
                    }
                }
                foreach (var device in currentDevices.ToArray())
                {
                    string guid = device.UniqueId;
                    if (!instances.Any(i => i == guid))
                    {
                        currentDevices.Remove(device);
                        device.Dispose();
                        Disconnected?.Invoke(this, new DeviceDisconnectedEventArgs(device));
                    }
                }
            }
        }

        private bool Match(DeviceItem deviceItem, params Usage[] usages)
        {
            return allDevices || deviceItem.Usages.GetAllValues().Any(u => usages.Any(usage => (Usage)u == usage));
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
            }
            disposed = true;
        }
    }
}
