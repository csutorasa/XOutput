using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Input;
using System;
using System.Linq;
using System.Threading;

namespace XOutput.Devices.Input.RawInput
{
    public class HIDSharp
    {
        public HIDSharp()
        {
            var local = DeviceList.Local;
            var devices = local.GetDevices(DeviceTypes.Hid)
                .OfType<HidDevice>();
            foreach (var device in devices)
            {
                HidStream hidStream;
                if (device.TryOpen(out hidStream))
                {
                    hidStream.ReadTimeout = Timeout.Infinite;
                    var reportDescriptor = device.GetReportDescriptor();

                    foreach (var deviceItem in reportDescriptor.DeviceItems)
                    {
                        if (!deviceItem.Usages.GetAllValues().Any(u => (Usage)u == Usage.GenericDesktopGamepad || (Usage)u == Usage.GenericDesktopJoystick || (Usage)u == Usage.GenericDesktopMultiaxisController))
                        {
                            continue;
                        }

                        var outputReport = new byte[device.GetMaxOutputReportLength()];

                        using (hidStream)
                        {
                            var inputReportBuffer = new byte[device.GetMaxInputReportLength()];
                            var inputReceiver = reportDescriptor.CreateHidDeviceInputReceiver();
                            var inputParser = deviceItem.CreateDeviceItemInputParser();
                            inputReceiver.Start(hidStream);

                            int startTime = Environment.TickCount;
                            while (true)
                            {
                                if (!inputReceiver.IsRunning) { break; }

                                Report report;
                                while (inputReceiver.TryRead(inputReportBuffer, 0, out report))
                                {
                                    if (inputParser.TryParseReport(inputReportBuffer, 0, report))
                                    {
                                        WriteDeviceItemInputParserResult(inputParser);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static void WriteDeviceItemInputParserResult(DeviceItemInputParser parser)
        {
            while (parser.HasChanged)
            {
                int changedIndex = parser.GetNextChangedIndex();
                var previousDataValue = parser.GetPreviousValue(changedIndex);
                var dataValue = parser.GetValue(changedIndex);

                Console.WriteLine(string.Format("  {0}: {1} -> {2}", (Usage)dataValue.Usages.FirstOrDefault(), previousDataValue.GetPhysicalValue(), dataValue.GetPhysicalValue()));
            }
        }
    }
}
