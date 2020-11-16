using NLog;
using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using XOutput.Core.Threading;

namespace XOutput.Devices.Input.DirectInput
{
    public class DirectInputDevice : IInputDevice
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private static readonly Regex hidRegex = new Regex("(hid)#([^#]+)#([^#]+)");

        public IEnumerable<InputSource> Sources => sources;

        public IEnumerable<ForceFeedbackTarget> ForceFeedbacks => targets;

        public InputConfig InputConfiguration { get; set; }

        public InputDeviceMethod InputMethod => InputDeviceMethod.DirectInput;
        public string DisplayName { get; private set; }
        public string InterfacePath { get; private set; }
        public string UniqueId { get; private set; }
        public string HardwareID { get; private set; }
        public bool Running => readThreadContext?.Running ?? false;

        public event DeviceInputChangedHandler InputChanged;
        private readonly DeviceInputChangedEventArgs inputChangedEventArgs;

        private ThreadContext readThreadContext;
        private ThreadContext forceFeedbackThreadContext;
        private readonly DirectInputSource[] sources;
        private readonly ForceFeedbackTarget[] targets;
        private readonly Dictionary<ForceFeedbackTarget, DirectDeviceForceFeedback> forceFeedbacks;
        private readonly Joystick joystick;
        private readonly InputConfigManager inputConfigManager;
        private bool disposed = false;

        public DirectInputDevice(InputConfigManager inputConfigManager, Joystick joystick, string guid, string productName, bool hasForceFeedbackDevice,
            string uniqueId, string hardwareId, string interfacePath)
        {
            this.inputConfigManager = inputConfigManager;
            this.joystick = joystick;
            UniqueId = uniqueId;
            InterfacePath = interfacePath;
            HardwareID = hardwareId;
            DisplayName = productName;
            var buttonObjectInstances = joystick.GetObjects(DeviceObjectTypeFlags.Button).Where(b => b.Usage > 0).OrderBy(b => b.ObjectId.InstanceNumber).Take(128).ToArray();
            var buttons = buttonObjectInstances.Select((b, i) => DirectInputSource.FromButton(this, b, i)).ToArray();
            var axes = GetAxes().OrderBy(a => a.Usage).Take(24).Select(a => DirectInputSource.FromAxis(this, a));
            var sliders = joystick.GetObjects().Where(o => o.ObjectType == ObjectGuid.Slider).OrderBy(a => a.Usage).Select((s, i) => DirectInputSource.FromSlider(this, s, i));
            IEnumerable<DirectInputSource> dpads = new DirectInputSource[0];
            if (joystick.Capabilities.PovCount > 0)
            {
                dpads = Enumerable.Range(0, joystick.Capabilities.PovCount)
                    .SelectMany(i => DirectInputSource.FromDPad(this, i));
            }
            sources = buttons.Concat(axes).Concat(sliders).Concat(dpads).ToArray();

            EffectInfo force = null;
            if (hasForceFeedbackDevice)
            {
                try
                {
                    joystick.SetCooperativeLevel(WindowHandleStore.Handle, CooperativeLevel.Background | CooperativeLevel.Exclusive);
                }
                catch (Exception)
                {
                    logger.Warn($"Failed to set cooperative level to exclusive for {ToString()}");
                }
                var constantForce = joystick.GetEffects().FirstOrDefault(x => x.Guid == EffectGuid.ConstantForce);
                if (constantForce == null)
                {
                    force = joystick.GetEffects().FirstOrDefault();
                }
                else
                {
                    force = constantForce;
                }
                var actuatorAxes = joystick.GetObjects().Where(doi => doi.ObjectId.Flags.HasFlag(DeviceObjectTypeFlags.ForceFeedbackActuator)).ToArray();
                targets = actuatorAxes.Select(i => new ForceFeedbackTarget(this, i.Name, i.Offset)).ToArray();
                forceFeedbacks = targets.ToDictionary(t => t, t => new DirectDeviceForceFeedback(joystick, UniqueId, force, actuatorAxes.First(a => a.Offset == t.Offset)));
            }
            else
            {
                targets = new ForceFeedbackTarget[0];
                forceFeedbacks = new Dictionary<ForceFeedbackTarget, DirectDeviceForceFeedback>();
            }
            joystick.Acquire();
            inputChangedEventArgs = new DeviceInputChangedEventArgs(this);
        }

        public void Start()
        {
            if (!Running)
            {
                InputConfiguration.Sources.ForEach(s => {
                    var source = FindSource(s.Offset);
                    if (source != null) {
                        source.Update(s);
                    }
                });
                readThreadContext = ThreadCreator.CreateLoop($"{DisplayName} DirectInput reader", ReadLoop, 1).Start();
                forceFeedbackThreadContext = ThreadCreator.CreateLoop($"{DisplayName} DirectInput force feedback", ForceFeedbackLoop, 10).Start();
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
                forceFeedbackThreadContext.Cancel().Wait();
                if (InputConfiguration.Autostart) {
                    InputConfiguration.Autostart = false;
                    inputConfigManager.SaveConfig(this);
                }
            }
        }

        private void ReadLoop()
        {
            JoystickState state = joystick.GetCurrentState();
            var changedSources = sources.Where(s => s.Refresh(state)).ToArray();
            inputChangedEventArgs.Refresh(changedSources);
            if (inputChangedEventArgs.ChangedValues.Any())
            {
                InputChanged?.Invoke(this, inputChangedEventArgs);
            }
        }

        private void ForceFeedbackLoop()
        {
            foreach (var forceFeedback in forceFeedbacks)
            {
                double targetValue = forceFeedback.Key.Value;
                forceFeedback.Value.Value = targetValue;
            }
        }

        public InputSource FindSource(int offset)
        {
            return sources.FirstOrDefault(d => d.Offset == offset);
        }

        public ForceFeedbackTarget FindTarget(int offset)
        {
            return targets.FirstOrDefault(d => d.Offset == offset);
        }

        private DeviceObjectInstance[] GetAxes()
        {
            var axes = joystick.GetObjects(DeviceObjectTypeFlags.AbsoluteAxis).Where(o => o.ObjectType != ObjectGuid.Slider).ToArray();
            foreach (var axis in axes)
            {
                var properties = joystick.GetObjectPropertiesById(axis.ObjectId);
                try
                {
                    properties.Range = new InputRange(ushort.MinValue, ushort.MaxValue);
                    properties.DeadZone = 0;
                    properties.Saturation = 10000;
                }
                catch (SharpDXException ex)
                {
                    logger.Error(ex);
                }
            }
            return axes;
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
                forceFeedbackThreadContext?.Cancel()?.Wait();
                joystick.Dispose();
            }
            disposed = true;
        }
    }
}
