using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Core.Threading;

namespace XOutput.Devices.Input.Mouse
{
    public class MouseDevice : IInputDevice
    {
        public IEnumerable<InputSource> Sources => sources;

        public IEnumerable<ForceFeedbackTarget> ForceFeedbacks => targets;

        public InputConfig InputConfiguration { get; set; }

        public string DisplayName { get; private set; }
        public string UniqueId { get; private set; }
        public string HardwareID { get; private set; }

        public event DeviceInputChangedHandler InputChanged;
        private readonly DeviceInputChangedEventArgs inputChangedEventArgs;

        private readonly ThreadContext readThreadContext;
        private readonly MouseSource[] sources;
        private readonly ForceFeedbackTarget[] targets;
        private readonly MouseHook hook;
        private bool disposed = false;

        public MouseDevice(MouseHook hook)
        {
            this.hook = hook;
            UniqueId = MouseDeviceProvider.DeviceId;
            DisplayName = MouseDeviceProvider.DeviceId;
            sources = Enum.GetValues(typeof(MouseButton)).OfType<MouseButton>().Select((b) => new MouseSource(this, b.ToString(), (int)b)).ToArray();
            targets = new ForceFeedbackTarget[0];
            inputChangedEventArgs = new DeviceInputChangedEventArgs(this);
            readThreadContext = ThreadCreator.CreateLoop($"{DisplayName} input reader", ReadLoop, 1).Start();
        }

        private void ReadLoop()
        {
            var changedSources = sources.Where(s => s.Refresh(hook.IsPressed((MouseButton)s.Offset))).ToList();
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
                readThreadContext.Cancel().Wait();
            }
            disposed = true;
        }
    }
}
