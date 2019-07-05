using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.Input;
using XOutput.Devices.Mapper;
using XOutput.Logging;

namespace XOutput.Tools
{
    /// <summary>
    /// Contains the settings that are persisted.
    /// </summary>
    public sealed class Settings
    {
        private const string LanguageKey = "Language";
        private const string CloseToTrayKey = "CloseToTray";
        private const string ShowAllKey = "ShowAll";
        private const string HidGuardianEnabledKey = "HidGuardianEnabled";
        private const string General = "General";
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(Settings));

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
            }
            return settings;
        }

        public Dictionary<string, InputConfig> Input { get; set; }
        public Dictionary<string, InputMapper> Mapping { get; set; }
        public bool CloseToTray { get; set; }
        public bool ShowAll { get; set; }
        public bool HidGuardianEnabled { get; set; }

        public Settings()
        {
            Input = new Dictionary<string, InputConfig>();
            Mapping = new Dictionary<string, InputMapper>();
        }

        /// <summary>
        /// Writes the settings into a file.
        /// </summary>
        /// <param name="filePath">Filepath of the settings file</param>
        public void Save(string filePath)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        /// <summary>
        /// Gets the mapper with the given deviceID. If the mapper is not saved in this settings, a new mapper will be returned.
        /// </summary>
        /// <param name="id">DeviceID</param>
        /// <returns></returns>
        public InputMapper GetMapper(string id)
        {
            if (!Mapping.ContainsKey(id))
            {
                Mapping[id] = new InputMapper();
            }
            return Mapping[id];
        }

        /// <summary>
        /// Gets the input configuration with the given deviceID. If configuration are not saved in this settings, a new configuration will be returned.
        /// </summary>
        /// <param name="id">DeviceID</param>
        /// <param name="initialValue">Default config</param>
        /// <returns></returns>
        public InputConfig GetInputConfiguration(string id, InputConfig initialValue)
        {
            if (initialValue == null)
            {
                Input[id] = new InputConfig();
                return Input[id];
            }
            if (!Input.ContainsKey(id))
            {
                Input[id] = initialValue;
                return Input[id];
            }
            InputConfig saved = Input[id];
            initialValue.ForceFeedback = saved.ForceFeedback;
            initialValue.StartWhenConnected = saved.StartWhenConnected;
            Input[id] = initialValue;
            return initialValue;
        }
    }
}
