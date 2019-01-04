using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices;
using XOutput.Devices.Input;
using XOutput.Devices.Input.DirectInput;
using XOutput.Devices.Input.Settings;
using XOutput.Devices.XInput;
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

        public Settings()
        {
            DirectInputDevices.Instance.DeviceConnected += Instance_DeviceConnected;
            DirectInputDevices.Instance.DeviceDisconnected += Instance_DeviceDisconnected;
        }

        private void Instance_DeviceDisconnected(object sender, DeviceDisconnectedEventArgs e)
        {
            IInputDevice inputDevice = sender as IInputDevice;
            InputDeviceSettings settings = InputDevices?.Where(id => id.Key == inputDevice.DisplayName).Select(id => id.Value).FirstOrDefault();
            if (settings != null)
            {
                settings.Device = null;
            }
        }

        private void Instance_DeviceConnected(object sender, DeviceConnectedEventArgs e)
        {
            IInputDevice inputDevice = sender as IInputDevice;
            string name = inputDevice is DirectDevice ? (inputDevice as DirectDevice).Id.ToString() : inputDevice.DisplayName;
            if (InputDevices != null)
            {
                InputDeviceSettings settings = InputDevices.Where(id => id.Key == name).Select(id => id.Value).FirstOrDefault();
                if (settings == null)
                {
                    var inputSettings = new Dictionary<InputType, InputSettings>();
                    foreach (var axis in inputDevice.Axes)
                    {
                        inputSettings[axis] = new InputSettings
                        {
                            Deadzone = 0,
                            AntiDeadzone = 0,
                        };
                    }
                    InputDevices[name] = new InputDeviceSettings
                    {
                        InputSettings = inputSettings,
                        Device = inputDevice,
                    };

                }
                else
                {
                    settings.Device = inputDevice;
                    foreach (var axis in inputDevice.Axes)
                    {
                        if (!settings.InputSettings.ContainsKey(axis))
                        {
                            settings.InputSettings[axis] = new InputSettings
                            {
                                Deadzone = 0,
                                AntiDeadzone = 0,
                            };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Reads a new setting from a file.
        /// </summary>
        /// <param name="filePath">Filepath of the settings file</param>
        /// <returns></returns>
        public static Settings Load(string filePath)
        {
            var settings = new Settings();
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new InputTypeConverter() },
            };
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath);
                settings = JsonConvert.DeserializeObject<Settings>(text);
            }
            if (settings.OutputDevices == null)
            {
                settings.OutputDevices = new Dictionary<string, OutputDeviceSettings>();
            }
            if (settings.InputDevices == null)
            {
                settings.InputDevices = new Dictionary<string, InputDeviceSettings>();
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
                    Mapping = new Dictionary<InputType, MapperSettings>()
                };
                foreach (var xInputType in XInputTypes.Values)
                {
                    settings.Mapping[xInputType] = new MapperSettings
                    {
                    };
                }
                OutputDevices[deviceName] = settings;
            }
            return OutputDevices[deviceName];
        }

        public void LoadInputs(IInputDevice inputDevice)
        {
            InputDeviceSettings settings = InputDevices.Where(id => id.Key == inputDevice.DisplayName).Select(id => id.Value).FirstOrDefault();
            if (settings == null)
            {
                InputDevices[inputDevice.DisplayName] = new InputDeviceSettings
                {
                    InputSettings = new Dictionary<InputType, InputSettings>(),
                    Device = inputDevice,
                };
            }
            else
            {
                settings.Device = inputDevice;
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
                    mapping.Value.InputType = device.Values.FirstOrDefault(it => it.ToString() == mapping.Value.Type);
                }
            }
        }

        public InputDeviceSettings GetInputSettings(string deviceId)
        {
            if (InputDevices.ContainsKey(deviceId))
            {
                return InputDevices[deviceId];
            }
            return null;
        }

        public void WriteInputs()
        {

        }

        public void WriteOutputs()
        {

        }
    }
}
