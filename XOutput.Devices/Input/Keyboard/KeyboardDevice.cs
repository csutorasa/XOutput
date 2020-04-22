using NLog;
using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using XOutput.Core.Threading;

namespace XOutput.Devices.Input.Keyboard
{
    public class KeyboardDevice : IInputDevice
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<InputSource> Sources => sources;

        public IEnumerable<ForceFeedbackTarget> ForceFeedbacks => targets;

        public InputConfig InputConfiguration { get; set; }

        public string DisplayName { get; private set; }
        public string UniqueId { get; private set; }
        public string HardwareID { get; private set; }

        public event DeviceInputChangedHandler InputChanged;
        private readonly DeviceInputChangedEventArgs inputChangedEventArgs;

        private readonly ThreadContext readThreadContext;
        private readonly KeyboardSource[] sources;
        private readonly ForceFeedbackTarget[] targets;
        private readonly KeyboardHook hook;
        private bool disposed = false;

        public KeyboardDevice(KeyboardHook hook)
        {
            this.hook = hook;
            UniqueId = KeyboardDeviceProvider.DeviceId;
            DisplayName = KeyboardDeviceProvider.DeviceId;
            sources = Enum.GetValues(typeof(KeyboardButton)).OfType<KeyboardButton>().Select((b) => new KeyboardSource(this, b.ToString(), (int)b)).ToArray();
            targets = new ForceFeedbackTarget[0];
            inputChangedEventArgs = new DeviceInputChangedEventArgs(this);
            readThreadContext = ThreadCreator.CreateLoop($"{DisplayName} input reader", ReadLoop, 1).Start();
        }

        private void ReadLoop()
        {
            var changedSources = sources.Where(s => s.Refresh(hook.IsPressed((KeyboardButton)s.Offset))).ToList();
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

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                readThreadContext.Cancel().Wait();
            }
            disposed = true;
        }
    }
}
