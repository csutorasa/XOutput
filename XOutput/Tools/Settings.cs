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
        private const string KeyboardKey = "Keyboard";
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

        public Dictionary<string, InputMapper> Inputs { get; set; }
        public bool CloseToTray { get; set; }
        public bool ShowAll { get; set; }
        public bool HidGuardianEnabled { get; set; }

        public Settings()
        {
            Inputs = new Dictionary<string, InputMapper>();
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
            if (!Inputs.ContainsKey(id))
            {
                if (id == KeyboardKey)
                {
                    Inputs[id] = new InputMapper();
                }
                else
                {
                    Inputs[id] = new InputMapper();
                }
            }
            return Inputs[id];
        }
    }
}
