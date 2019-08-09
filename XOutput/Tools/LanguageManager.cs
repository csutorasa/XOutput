using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using XOutput.Logging;

namespace XOutput.Tools
{
    /// <summary>
    /// Contains the language management.
    /// </summary>
    public sealed class LanguageManager
    {
        private readonly Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

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
            var assembly = Assembly.GetExecutingAssembly();
            var serializer = new JsonSerializer();
            foreach (var resourceName in assembly.GetManifestResourceNames().Where(s => s.StartsWith(assembly.GetName().Name + ".Resources.Languages.", StringComparison.CurrentCultureIgnoreCase))) {
                string resourceKey = resourceName.Split('.')[3];
                using (var stream = new JsonTextReader(new StreamReader(assembly.GetManifestResourceStream(resourceName))))
                {
                    data[resourceKey] = serializer.Deserialize<Dictionary<string, string>>(stream);
                }
                logger.Info(resourceKey + " language is loaded.");
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
