using NLog;
using System.Collections.Generic;
using System.Linq;
using XOutput.Common.Input;

namespace XOutput.Mapping.Input
{
    public class InputDevice
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public event InputDeviceInputChanged InputChanged;
        public event InputDeviceFeedback FeedbackReceived;

        public string Id => id;
        public string Name => name;
        public InputDeviceApi DeviceApi => deviceApi;

        private readonly string id;
        private readonly string name;
        private readonly InputDeviceApi deviceApi;
        private readonly IEnumerable<InputDeviceSourceWithValue> sources;
        private readonly IEnumerable<InputDeviceTargetWithValue> targets;

        public InputDevice(string id, string name, InputDeviceApi deviceApi, IEnumerable<InputDeviceSourceWithValue> sources, IEnumerable<InputDeviceTargetWithValue> targets)
        {
            this.id = id;
            this.name = name;
            this.deviceApi = deviceApi;
            this.sources = sources;
            this.targets = targets;
        }

        public InputDeviceSourceWithValue FindSource(int id)
        {
            return sources.FirstOrDefault(s => s.Id == id);
        }

        public List<InputDeviceSourceWithValue> FindAllSources()
        {
            return sources.ToList();
        }

        public InputDeviceTargetWithValue FindTarget(int id)
        {
            return targets.FirstOrDefault(s => s.Id == id);
        }

        public List<InputDeviceTargetWithValue> FindAllTargets()
        {
            return targets.ToList();
        }

        public void SetData(Dictionary<int, double> newValues)
        {
            var changedValues = new HashSet<InputDeviceSourceWithValue>();
            foreach (var newValue in newValues)
            {
                var source = FindSource(newValue.Key);
                if (source == null)
                {
                    logger.Warn($"Failed to find source {newValue.Key} for device {id}");
                    continue;
                }
                source.Value = newValue.Value;
                changedValues.Add(source);
            }
            InputChanged?.Invoke(this, new InputDeviceInputChangedEventArgs(changedValues));
        }

        public void SetFeedback(double smallMotor, double bigMotor)
        {
            var mappedTargets = new List<InputDeviceTargetWithValue>(); // TODO complete mapping for targets
            FeedbackReceived?.Invoke(this, new InputDeviceFeedbackEventArgs(mappedTargets));
        }
    }
}
