using System;
using System.Linq;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Threading;

namespace XOutput.Devices.Input.RawInput
{
    public class RawDevices
    {
        [ResolverMethod]
        public RawDevices()
        {
        }

        public void RegisterHook()
        {
            NativeMethods.RegisterDeviceHook(WindowHandleStore.Handle);
            WindowHandleStore.WindowEvent += WmEvent;
        }

        public void RemoveHook()
        {
            WindowHandleStore.WindowEvent -= WmEvent;
        }

        private void WmEvent(object sender, WindowEventArgs args)
        {
            NativeMethods.ReadData(args.LParam);
        }

        public static string GetHid(string vendor, string product)
        {
            int vendorId = Convert.ToInt32(vendor, 16);
            int productId = Convert.ToInt32(product, 16);
            return GetHid(vendorId, productId);
        }

        public static string GetHid(int vendorId, int productId)
        {
            var device = NativeMethods.GetDeviceList()
                .Where(d => d.DeviceType == RawInputDeviceType.HumanInterfaceDevice)
                .Select(d => d.DeviceHandle)
                .Select(NativeMethods.GetInfo)
                .Where(i => i.UsagePage == 1)
                .FirstOrDefault(i => i.VendorId == vendorId && i.ProductId == productId);
            return device?.ToHidString();
        }
    }
}
