using NLog;
using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XOutput.Devices.Input.DirectInput
{
    public class DirectInputDevice : IInputDevice
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<DPadDirection> DPads => throw new NotImplementedException();

        public IEnumerable<InputSource> Sources => sources;

        public IEnumerable<ForceFeedbackTarget> ForceFeedbacks => targets;

        public InputConfig InputConfiguration => throw new NotImplementedException();

        public string DisplayName { get; private set; }
        public string UniqueId { get; private set; }
        public string HardwareID { get; private set; }

        public event DeviceInputChangedHandler InputChanged;

        private readonly DirectInputSource[] sources;
        private readonly ForceFeedbackTarget[] targets;
        private readonly Joystick joystick;
        private bool disposed = false;

        public DirectInputDevice(Joystick joystick, string guid, string productName, bool hasForceFeedbackDevice, bool isHumanInterfaceDevice)
        {
            this.joystick = joystick;
            this.UniqueId = guid;
            this.DisplayName = productName;
            HardwareID = GetHid(joystick, isHumanInterfaceDevice);

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
            var actuatorAxes = joystick.GetObjects().Where(doi => doi.ObjectId.Flags.HasFlag(DeviceObjectTypeFlags.ForceFeedbackActuator)).ToArray();
            targets = actuatorAxes.Select(i => new ForceFeedbackTarget(this, i.Name, i.Offset)).ToArray();
        }

        public InputSource FindSource(int offset)
        {
            return sources.FirstOrDefault(d => d.Offset == offset);
        }

        public ForceFeedbackTarget FindTarget(int offset)
        {
            return targets.FirstOrDefault(d => d.Offset == offset);
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
                joystick.Dispose();
            }
            disposed = true;
        }
    }
}
