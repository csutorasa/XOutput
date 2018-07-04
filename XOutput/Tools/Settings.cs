using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.Input;
using XOutput.Devices.Input.Settings;
using XOutput.Devices.XInput.Settings;
using XOutput.Logging;

namespace XOutput.Tools
{
    /// <summary>
    /// Contains the settings that are persisted.
    /// </summary>
    public sealed class Settings
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(Settings));

        private static Settings instance;
        public static Settings Instance => instance;

        public bool CloseToTray { get; set; }
        public bool ShowAllDevices { get; set; }
        public string Lanugage
        {
            get => LanguageManager.Instance.Language;
            set { LanguageManager.Instance.Language = value; }
        }
        public Dictionary<string, InputDeviceSettings> InputDevices { get; set; }
        public Dictionary<string, OutputDeviceSettings> OutputDevices { get; set; }

        /// <summary>
        /// Reads a new setting from a file.
        /// </summary>
        /// <param name="filePath">Filepath of the settings file</param>
        /// <returns></returns>
        public static Settings Load(string filePath)
        {
            var settings = new Settings();
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath);
                settings = JsonConvert.DeserializeObject<Settings>(text);
                if (settings.OutputDevices == null)
                {
                    settings.OutputDevices = new Dictionary<string, OutputDeviceSettings>();
                }
                if (settings.InputDevices == null)
                {
                    settings.InputDevices = new Dictionary<string, InputDeviceSettings>();
                }
            }
            instance = settings;
            return settings;
        }

        /// <summary>
        /// Writes the settings into a file.
        /// </summary>
        /// <param name="filePath">Filepath of the settings file</param>
        public void Save(string filePath)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(this, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        public OutputDeviceSettings GetDeviceSettings(string deviceName)
        {
            if (!OutputDevices.ContainsKey(deviceName))
            {
                var settings = new OutputDeviceSettings
                {
                    StartWhenConnected = false,
                    DPadSettings = new DPadSettings(),
                    ForceFeedbackDevices = new List<ForceFeedbackSettings>(),
                    Mapping = new Dictionary<Devices.XInput.XInputTypes, MapperSettings>()
                };
                foreach (var xInputType in Devices.XInput.XInputHelper.Instance.Values)
                {
                    settings.Mapping[xInputType] = new MapperSettings
                    {
                        InputType = xInputType,
                    };
                }
                OutputDevices[deviceName] = settings;
            }
            return OutputDevices[deviceName];
        }

        public void LoadInputs(List<IInputDevice> inputDevices)
        {
            foreach (var inputDevice in InputDevices)
            {
                inputDevice.Value.Device = inputDevices.FirstOrDefault(id => id.DisplayName == inputDevice.Key);
            }
        }

        public void LoadOutputs(List<IInputDevice> inputDevices)
        {
            foreach (var outputDevice in OutputDevices)
            {
                outputDevice.Value.DPadSettings.Device = inputDevices.FirstOrDefault(id => id.DisplayName == outputDevice.Value.DPadSettings.DeviceName);
                foreach (var forceFeedbackDevice in outputDevice.Value.ForceFeedbackDevices)
                {
                    forceFeedbackDevice.Device = inputDevices.FirstOrDefault(id => id.DisplayName == forceFeedbackDevice.DeviceName);
                }
                foreach (var mapping in outputDevice.Value.Mapping)
                {
                    var device = inputDevices.FirstOrDefault(id => id.DisplayName == mapping.Value.DeviceName);
                    mapping.Value.Device = device;
                    mapping.Value.InputType = device.Axes.Concat(device.Buttons).Concat(device.Sliders).FirstOrDefault(it => it.ToString() == mapping.Value.Type);
                }
            }
        }
    }
}
