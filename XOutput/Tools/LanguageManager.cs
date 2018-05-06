using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private static LanguageManager instance = new LanguageManager("languages.txt");
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

        private LanguageManager(string filePath)
        {
            data = IniData.Deserialize(Properties.Resources.languages).Content;
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
