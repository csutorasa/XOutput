using SlimDX.DirectInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input.Mapper;

namespace XOutput
{
    /// <summary>
    /// Contains the language management.
    /// </summary>
    public sealed class LanguageManager
    {
        private Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

        private static LanguageManager instance = new LanguageManager("languages.txt");
        public static LanguageManager getInstance()
        {
            return instance;
        }

        private string language;
        public string Language
        {
            get { return language; }
            set
            {
                if (language != value)
                {
                    language = value;
                    LanguageModel.getInstance().Data = data[language];
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
