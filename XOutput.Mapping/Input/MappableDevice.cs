using NLog;
using System.Collections.Generic;
using System.Linq;

namespace XOutput.Mapping.Input
{
    public class MappableDevice
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public event MappableDeviceInputChanged InputChanged;
        public event MappableDeviceFeedback FeedbackReceived;

        public string Id => id;
        public string Name => name;

        private readonly string id;
        private readonly string name;
        private readonly IEnumerable<MappableSource> sources;

        public MappableDevice(string id, string name, IEnumerable<MappableSource> sources)
        {
            this.id = id;
            this.name = name;
            this.sources = sources;
        }

        public MappableSource FindSource(int id)
        {
            return sources.FirstOrDefault(s => s.Id == id);
        }

        public void SetData(Dictionary<int, double> newValues)
        {
            var changedValues = new HashSet<MappableSource>();
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
            InputChanged?.Invoke(this, new MappableDeviceInputChangedEventArgs(changedValues));
        }

        public void SetFeedback(double smallMotor, double bigMotor)
        {
            FeedbackReceived?.Invoke(this, new MappableDeviceFeedbackEventArgs(smallMotor, bigMotor));
        }
    }
}
