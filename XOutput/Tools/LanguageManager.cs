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
        public static LanguageManager Instance => instance;

        private string language;
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

        public IEnumerable<string> GetLanguages()
        {
            return data.Keys;
        }
    }
}
