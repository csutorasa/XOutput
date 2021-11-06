using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Emulation;
using XOutput.Mapping.Input;
using XOutput.Mapping.Mapper;

namespace XOutput.Mapping.Controller
{
    public abstract class ControllerBase<T> : IEmulatedController where T : struct, Enum
    {
        public string Id { get; }
        public string Name { get; }
        public abstract IDevice Device { get; }

        private Dictionary<T, Func<double>> inputGetters = new Dictionary<T, Func<double>>();
        private List<Action<double, double>> forceFeedbackSetters = new List<Action<double, double>>();
        private readonly List<InputDevice> boundDevices = new List<InputDevice>();

        public void Configure(ControllerConfig<T> config, IEnumerable<InputDevice> devices)
        {
            var mapping = config.InputMapping;
            foreach (var input in Enum.GetValues(typeof(T)).OfType<T>().Where(i => !mapping.ContainsKey(i)))
            {
                mapping[input] = new InputMapperCollection();
            }
            foreach (var device in boundDevices)
            {
                device.InputChanged -= InputDeviceChanged;
            }
            forceFeedbackSetters.Clear();
            boundDevices.Clear();
            var deviceLookup = devices.ToDictionary(d => d.Id, d => d);
            inputGetters = mapping.ToDictionary(m => m.Key, m => CreateGetter(deviceLookup, m.Value, GetDefaultValue(m.Key)));
            forceFeedbackSetters = config.ForceFeedbackMapping.Select(m => CreateSetter(deviceLookup, m)).ToList();
            foreach (var device in devices)
            {
                device.InputChanged += InputDeviceChanged;
                boundDevices.Add(device);
            }
        }

        protected void InputDeviceChanged(object sender, InputDeviceInputChangedEventArgs e)
        {
            InputChanged(e);
        }

        protected abstract void InputChanged(InputDeviceInputChangedEventArgs args);

        protected abstract double GetDefaultValue(T input);

        public abstract void Stop();

        public Dictionary<T, double> GetValues()
        {
            return Enum.GetValues<T>().ToDictionary((input) => input, (input) => GetValue(input));
        }

        public Dictionary<string, double> GetSources()
        {
            return Enum.GetValues<T>().ToDictionary((input) => input.ToString(), (input) => GetValue(input));
        }

        public Dictionary<string, double> GetTargets()
        {
            return Enum.GetValues<T>().ToDictionary((input) => input.ToString(), (input) => GetValue(input));
        }

        protected double GetValue(T input)
        {
            return inputGetters.ContainsKey(input) ? inputGetters[input]() : GetDefaultValue(input);
        }

        protected bool GetBoolValue(T input)
        {
            return GetValue(input) > 0.5;
        }

        protected void SetForceFeedback(double big, double small)
        {
            forceFeedbackSetters.ForEach(s => s(big, small));
        }

        private Func<double> CreateGetter(Dictionary<string, InputDevice> deviceLookup, InputMapperCollection collection, double defaultValue)
        {
            var sources = collection.Mappers
                .Where(m => deviceLookup.ContainsKey(m.Device))
                .Select(m => deviceLookup[m.Device].FindSource(m.InputId))
                .Where(s => s != null)
                .ToArray();
            if (sources.Length == 0)
            {
                return () => defaultValue;
            }
            return () =>
            {
                return collection.GetValue(sources.Select(s => s.Value));
            };
        }

        private Action<double, double> CreateSetter(Dictionary<string, InputDevice> deviceLookup, ForceFeedbackMapper mapper)
        {
            if (deviceLookup.ContainsKey(mapper.Device))
            {
                var target = deviceLookup[mapper.Device];
                if (target != null)
                {
                    if (mapper.Big)
                    {
                        // target.SetFeedback(0, 0);
                    }
                    else
                    {
                        // target.SetFeedback(0, 0);
                    }
                }
            }
            return (big, small) => { };
        }
    }
}
