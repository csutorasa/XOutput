using HidSharp;
using HidSharp.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using XOutput.Core.DependencyInjection;

namespace XOutput.Devices.Input.RawInput
{
    public class RawInputDeviceProvider : IInputDeviceProvider
    {
        public event DeviceConnectedHandler Connected;
        public event DeviceDisconnectedHandler Disconnected;

        private readonly IgnoredDeviceService ignoredDeviceService;
        private readonly InputConfigManager inputConfigManager;
        private readonly List<IInputDevice> currentDevices = new List<IInputDevice>();
        private readonly object lockObject = new object();
        private bool disposed = false;
        private bool allDevices = false;

        [ResolverMethod]
        public RawInputDeviceProvider(IgnoredDeviceService ignoredDeviceService, InputConfigManager inputConfigManager)
        {
            this.ignoredDeviceService = ignoredDeviceService;
            this.inputConfigManager = inputConfigManager;
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
            lock (lockObject)
            {
                var local = DeviceList.Local;
                var devices = local.GetDevices(DeviceTypes.Hid).OfType<HidDevice>();
                List<string> instances = new List<string>();
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
                                var inputDevice = new RawInputDevice(device, reportDescriptor, deviceItem, hidStream, uniqueId);
                                var config = inputConfigManager.LoadConfig(inputDevice.UniqueId);
                                inputDevice.InputConfiguration = config;
                                currentDevices.Add(inputDevice);
                                Connected?.Invoke(this, new DeviceConnectedEventArgs(inputDevice));
                                    
                            }
                        }
                    }
                    catch (Exception)
                    {

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
