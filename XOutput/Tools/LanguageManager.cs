using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private static LanguageManager instance = new LanguageManager(typeof(Properties.Languages));
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
                if (language != value)
                {
                    language = value;
                    logger.Info("Language is set to " + language);
                    LanguageModel.Instance.Data = data[language];
                }
            }
        }

        private LanguageManager(Type resources)
        {
            data = new Dictionary<string, Dictionary<string, string>>();
            using (var resourceSet = new ResourceManager(resources).GetResourceSet(CultureInfo.CurrentUICulture, true, true))
            {
                foreach (DictionaryEntry x in resourceSet)
                {
                    try
                    {
                        var language = (x.Key as string).Replace("_", " ");
                        var content = Encoding.UTF8.GetString(x.Value as byte[]);
                        var translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                        data[language] = translations;
                    }
                    catch (Exception e)
                    {
                        logger.Error("Failed to load language " + language, e);
                    }
                }
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
