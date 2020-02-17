using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Devices.Input;
using XOutput.Devices.Mapper;

namespace XOutput.Devices.Controller
{
    public abstract class ControllerBase<T> where T : struct, IConvertible
    {
        private Dictionary<T, Func<double>> inputGetters = new Dictionary<T, Func<double>>();
        private List<Action<double, double>> forceFeedbackSetters = new List<Action<double, double>>();
        private List<IInputDevice> boundDevices = new List<IInputDevice>();

        public void Configure(ControllerConfig<T> config, IEnumerable<IInputDevice> devices)
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
            boundDevices.Clear();
            var deviceLookup = devices.ToDictionary(d => d.UniqueId, d => d);
            inputGetters = mapping.ToDictionary(m => m.Key, m => CreateGetter(deviceLookup, m.Value, GetDefaultValue(m.Key)));
            foreach (var device in devices)
            {
                device.InputChanged += InputDeviceChanged;
                boundDevices.Add(device);
            }
        }

        protected abstract void InputDeviceChanged(object sender, DeviceInputChangedEventArgs e);

        protected abstract double GetDefaultValue(T input);

        protected double GetValue(T input)
        {
            return inputGetters.ContainsKey(input) ? inputGetters[input]() : GetDefaultValue(input);
        }

        protected void SetForceFeedback(double big, double small)
        {
            forceFeedbackSetters.ForEach(s => s(big, small));
        }

        private Func<double> CreateGetter(Dictionary<string, IInputDevice> deviceLookup, InputMapperCollection collection, double defaultValue)
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
                return collection.GetValue(sources.Select(s => s.GetValue()));
            };
        }

        private Action<double, double> CreateSetter(Dictionary<string, IInputDevice> deviceLookup, ForceFeedbackMapper mapper)
        {
            if(deviceLookup.ContainsKey(mapper.Device))
            {
                var target = deviceLookup[mapper.Device].FindTarget(mapper.InputId);
                if (target != null)
                {
                    if (mapper.Big)
                    {
                        return (big, small) => { target.SetValue(big); };
                    }
                    else
                    {
                        return (big, small) => { target.SetValue(small); };
                    }
                }
            }
            return (big, small) => { };
        }
    }
}
