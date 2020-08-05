using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Core.Threading;

namespace XOutput.Devices.Input.Keyboard
{
    public class KeyboardDevice : IInputDevice
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<InputSource> Sources => sources;

        public IEnumerable<ForceFeedbackTarget> ForceFeedbacks => targets;

        public InputConfig InputConfiguration { get; set; }

        public InputDeviceMethod InputMethod => InputDeviceMethod.WindowsApi;
        public string InterfacePath { get; private set; }
        public string DisplayName { get; private set; }
        public string UniqueId { get; private set; }
        public string HardwareID { get; private set; }
        public bool Running => readThreadContext?.Running ?? false;

        public event DeviceInputChangedHandler InputChanged;
        private readonly DeviceInputChangedEventArgs inputChangedEventArgs;

        private ThreadContext readThreadContext;
        private readonly KeyboardSource[] sources;
        private readonly ForceFeedbackTarget[] targets;
        private readonly KeyboardHook hook;
        private readonly InputConfigManager inputConfigManager;
        private bool disposed = false;

        public KeyboardDevice(InputConfigManager inputConfigManager, KeyboardHook hook)
        {
            this.inputConfigManager = inputConfigManager;
            this.hook = hook;
            UniqueId = KeyboardDeviceProvider.DeviceId;
            DisplayName = KeyboardDeviceProvider.DeviceId;
            sources = Enum.GetValues(typeof(KeyboardButton)).OfType<KeyboardButton>().Select((b) => new KeyboardSource(this, b.ToString(), (int)b)).ToArray();
            targets = new ForceFeedbackTarget[0];
            inputChangedEventArgs = new DeviceInputChangedEventArgs(this);
        }

        public void Start()
        {
            if (!Running)
            {
                hook.StartHook();
                readThreadContext = ThreadCreator.CreateLoop($"{DisplayName} input reader", ReadLoop, 1).Start();
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
