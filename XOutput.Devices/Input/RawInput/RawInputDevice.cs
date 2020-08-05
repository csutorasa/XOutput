using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using XOutput.Core.Threading;

namespace XOutput.Devices.Input.RawInput
{
    public class RawInputDevice : IInputDevice
    {
        private const int limit = 10;

        public IEnumerable<InputSource> Sources => sources;

        public IEnumerable<ForceFeedbackTarget> ForceFeedbacks => targets;

        public InputConfig InputConfiguration { get; set; }

        public InputDeviceMethod InputMethod => InputDeviceMethod.RawInput;
        public string DisplayName { get; private set; }
        public string InterfacePath { get; private set; }
        public string UniqueId { get; private set; }
        public string HardwareID { get; private set; }
        public bool Running => readThreadContext?.Running ?? false;

        public event DeviceInputChangedHandler InputChanged;

        private readonly DeviceInputChangedEventArgs inputChangedEventArgs;
        private readonly ForceFeedbackTarget[] targets = new ForceFeedbackTarget[0];
        private readonly HidDevice device;
        private readonly HidDeviceInputReceiver inputReceiver;
        private readonly DeviceItemInputParser inputParser;
        private readonly InputConfigManager inputConfigManager;
        private byte[] inputReportBuffer;
        private readonly RawInputSource[] sources;
        private bool disposed = false;
        private ThreadContext readThreadContext;

        public RawInputDevice(InputConfigManager inputConfigManager, HidDevice device, ReportDescriptor reportDescriptor, DeviceItem deviceItem,
            HidStream hidStream, string uniqueId)
        {
            this.inputConfigManager = inputConfigManager;
            this.device = device;
            inputReportBuffer = new byte[device.GetMaxInputReportLength()];
            inputReceiver = reportDescriptor.CreateHidDeviceInputReceiver();
            inputParser = deviceItem.CreateDeviceItemInputParser();
            inputReceiver.Start(hidStream);
            DisplayName = device.GetProductName();
            UniqueId = uniqueId;
            InterfacePath = device.DevicePath;
            HardwareID = IdHelper.GetHardwareId(InterfacePath);
            inputChangedEventArgs = new DeviceInputChangedEventArgs(this);
            sources = reportDescriptor.InputReports.SelectMany(ir => ir.DataItems)
                .SelectMany(di => di.Usages.GetAllValues())
                .Select(u => (Usage)u)
                .SelectMany(u => RawInputSource.FromUsage(this, u))
                .ToArray();
        }

        public void Start()
        {
            if (!Running)
            {
                readThreadContext = ThreadCreator.CreateLoop($"{DisplayName} RawInput reader", ReadLoop, 1).Start();
                if (!InputConfiguration.Autostart) {
                    InputConfiguration.Autostart = true;
                    inputConfigManager.SaveConfig(this);
                }
            }
        }
        
        public void Stop()
        {
            if (Running)
            {
                readThreadContext.Cancel().Wait();
                if (InputConfiguration.Autostart) {
                    InputConfiguration.Autostart = false;
                    inputConfigManager.SaveConfig(this);
                }
            }
        }

        private void ReadLoop()
        {
            if (!inputReceiver.IsRunning) {
                return;
            }

            Report report;
            Dictionary<Usage, DataValue> changedIndexes = new Dictionary<Usage, DataValue>();
            for (int i = 0; i < limit && inputReceiver.TryRead(inputReportBuffer, 0, out report); i++)
            {
                if (inputParser.TryParseReport(inputReportBuffer, 0, report))
                {
                    while (inputParser.HasChanged)
                    {
                        int changedIndex = inputParser.GetNextChangedIndex();
                        var dataValue = inputParser.GetValue(changedIndex);
                        if (dataValue.Usages.Count() > 0 ) {
                            changedIndexes[(Usage)dataValue.Usages.FirstOrDefault()] = dataValue;
                        }
                    }
                }
            }
            var changedSources = sources.Where(s => s.Refresh(changedIndexes)).ToArray();
            inputChangedEventArgs.Refresh(changedSources);
            if (inputChangedEventArgs.ChangedValues.Any())
            {
                InputChanged?.Invoke(this, inputChangedEventArgs);
            }
        }


        public InputSource FindSource(int offset)
        {
            return sources.FirstOrDefault(d => d.Offset == offset);
        }

        public ForceFeedbackTarget FindTarget(int offset)
        {
            return null;
        }

        public void SetForceFeedback(double big, double small)
        {

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                readThreadContext?.Cancel()?.Wait();
            }
            disposed = true;
        }
    }
}
