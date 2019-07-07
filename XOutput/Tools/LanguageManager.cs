using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using XOutput.Logging;

namespace XOutput.Tools
{
    /// <summary>
    /// Contains the language management.
    /// </summary>
    public sealed class LanguageManager
    {
        private Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(LanguageManager));
        private static LanguageManager instance = new LanguageManager();
        /// <summary>
        /// Gets the singleton instance of the class.
        /// </summary>
        public static LanguageManager Instance => instance;

        private string language;
        /// <summary>
        /// Gets or sets the current language.
        /// </summary>
        public string Language
        {
            get { return language; }
            set
            {
                var v = value;
                if (!data.ContainsKey(v))
                {
                    v = "English";
                }
                if (language != v)
                {
                    language = v;
                    logger.Info("Language is set to " + language);
                    LanguageModel.Instance.Data = data[language];
                }
            }
        }

        private LanguageManager()
        {
            ResourceSet resourceSet = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                string resourceKey = entry.Key.ToString().Replace("_", " ");
                string value = entry.Value as string;
                data[resourceKey] = JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
            }
            Language = "English";
        }

        /// <summary>
        /// Gets the available languages.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetLanguages()
        {
            return data.Keys;
        }
    }
}
