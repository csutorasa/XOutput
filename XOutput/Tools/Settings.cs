using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                IniData ini = IniData.Deserialize(text);
                foreach (var section in ini.Content)
                {
                    var id = section.Key;
                    if (id == General)
                    {
                        if (section.Value.ContainsKey(LanguageKey))
                        {
                            LanguageManager.Instance.Language = section.Value[LanguageKey];
                        }
                        if (section.Value.ContainsKey(CloseToTrayKey))
                        {
                            settings.CloseToTray = section.Value[CloseToTrayKey] == "true";
                        }
                    }
                    else if (id == KeyboardKey)
                    {
                        settings.mappers[id] = KeyboardToXInputMapper.Parse(section.Value);
                        logger.Debug("Mapper loaded for keyboard");
                    }
                    else
                    {
                        settings.mappers[id] = DirectToXInputMapper.Parse(section.Value);
                        logger.Debug("Mapper loaded for " + id);
                    }
                }
            }
            return settings;
        }

        private Dictionary<string, InputMapperBase> mappers;
        public bool CloseToTray { get; set; }
        public bool ShowAll { get; set; }

        public Settings()
        {
            mappers = new Dictionary<string, InputMapperBase>();
        }

        /// <summary>
        /// Writes the settings into a file.
        /// </summary>
        /// <param name="filePath">Filepath of the settings file</param>
        public void Save(string filePath)
        {
            IniData ini = new IniData();
            foreach (var mapper in mappers)
            {
                ini.AddSection(mapper.Key, mapper.Value.ToDictionary());
            }
            Dictionary<string, string> generalSettings = new Dictionary<string, string>();
            generalSettings[LanguageKey] = LanguageManager.Instance.Language;
            generalSettings[CloseToTrayKey] = CloseToTray ? "true" : "false";
            generalSettings[ShowAllKey] = ShowAll ? "true" : "false";
            ini.AddSection(General, generalSettings);
            File.WriteAllText(filePath, ini.Serialize());
        }

        /// <summary>
        /// Gets the mapper with the given deviceID. If the mapper is not saved in this settings, a new mapper will be returned.
        /// </summary>
        /// <param name="id">DeviceID</param>
        /// <returns></returns>
        public InputMapperBase GetMapper(string id)
        {
            if (!mappers.ContainsKey(id))
            {
                if (id == KeyboardKey)
                {
                    mappers[id] = new KeyboardToXInputMapper();
                }
                else
                {
                    mappers[id] = new DirectToXInputMapper();
                }
            }
            return mappers[id];
        }
    }
}
