using NLog;
using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using XOutput.Core.Threading;

namespace XOutput.Devices.Input.DirectInput
{
    public class DirectInputDevice : IInputDevice
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
        private readonly ThreadContext forceFeedbackThreadContext;
        private readonly MouseSource[] sources;
        private readonly ForceFeedbackTarget[] targets;
        private readonly Dictionary<ForceFeedbackTarget, DirectDeviceForceFeedback> forceFeedbacks;
        private readonly Joystick joystick;
        private bool disposed = false;

        public DirectInputDevice(Joystick joystick, string guid, string productName, bool hasForceFeedbackDevice, bool isHumanInterfaceDevice)
        {
            this.joystick = joystick;
            this.UniqueId = guid;
            this.DisplayName = productName;
            HardwareID = GetHid(joystick, isHumanInterfaceDevice);

            var buttonObjectInstances = joystick.GetObjects(DeviceObjectTypeFlags.Button).Where(b => b.Usage > 0).OrderBy(b => b.ObjectId.InstanceNumber).Take(128).ToArray();
            var buttons = buttonObjectInstances.Select((b, i) => MouseSource.FromButton(this, b, i)).ToArray();
            var axes = GetAxes().OrderBy(a => a.Usage).Take(24).Select(a => MouseSource.FromAxis(this, a));
            var sliders = joystick.GetObjects().Where(o => o.ObjectType == ObjectGuid.Slider).OrderBy(a => a.Usage).Select((s, i) => MouseSource.FromSlider(this, s, i));
            IEnumerable<MouseSource> dpads = new MouseSource[0];
            if (joystick.Capabilities.PovCount > 0)
            {
                dpads = Enumerable.Range(0, joystick.Capabilities.PovCount)
                    .SelectMany(i => MouseSource.FromDPad(this, i));
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
                forceFeedbacks = targets.ToDictionary(t => t, t => new DirectDeviceForceFeedback(joystick, force,actuatorAxes.First(a => a.Offset == t.Offset)));
            } else
            {
                targets = new ForceFeedbackTarget[0];
                forceFeedbacks = new Dictionary<ForceFeedbackTarget, DirectDeviceForceFeedback>();
            }
            joystick.Acquire();
            inputChangedEventArgs = new DeviceInputChangedEventArgs(this);
            readThreadContext = ThreadCreator.CreateLoop($"{DisplayName} input reader", ReadLoop, 1).Start();
            forceFeedbackThreadContext = ThreadCreator.CreateLoop($"{DisplayName} force feedback", ForceFeedbackLoop, 10).Start();
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

        public void SetForceFeedback(double big, double small)
        {
            if (InputConfiguration != null)
            {
                foreach (var target in InputConfiguration.BigMotors.Select(o => FindTarget(o)).Where(t => t != null))
                {
                    target.Value = big;
                }
                foreach (var target in InputConfiguration.SmallMotors.Select(o => FindTarget(o)).Where(t => t != null))
                {
                    target.Value = small;
                }
            }
        }

        private string GetHid(Joystick joystick, bool isHumanInterfaceDevice)
        {
            if (isHumanInterfaceDevice)
            {
                string path = joystick.Properties.InterfacePath;
                if (path.Contains("hid#"))
                {
                    path = path.Substring(path.IndexOf("hid#"));
                    path = path.Replace('#', '\\');
                    int first = path.IndexOf('\\');
                    int second = path.IndexOf('\\', first + 1);
                    if (second > 0)
                    {
                        return path.Remove(second).ToUpper();
                    }
                }
            }
            return null;
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

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                forceFeedbackThreadContext.Cancel().Wait();
                readThreadContext.Cancel().Wait();
                joystick.Dispose();
            }
            disposed = true;
        }
    }
}
