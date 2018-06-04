using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Dictionary<string, OutputDeviceSettings> OutputDevices { get; set; }
        public string Lanugage
        {
            get => LanguageManager.Instance.Language;
            set { LanguageManager.Instance.Language = value; }
        }

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
            instance = settings;
            return settings;
        }

        /// <summary>
        /// Writes the settings into a file.
        /// </summary>
        /// <param name="filePath">Filepath of the settings file</param>
        public void Save(string filePath)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(this));
        }

        public OutputDeviceSettings GetDeviceSettings(string deviceName)
        {
            if (!OutputDevices.ContainsKey(deviceName))
            {
                OutputDevices[deviceName] = new OutputDeviceSettings();
            }
            return OutputDevices[deviceName];
        }
    }
}
