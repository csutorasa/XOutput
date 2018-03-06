using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XOutput.UI;

namespace XOutput
{
    /// <summary>
    /// Contains the language management.
    /// </summary>
    public sealed class LanguageModel : ModelBase
    {
        private static LanguageModel instance = new LanguageModel();
        public static LanguageModel getInstance()
        {
            return instance;
        }

        private Dictionary<string, string> data;
        public Dictionary<string, string> Data {
            get { return data; }
            set
            {
                if(data != value)
                {
                    data = value;
                    OnPropertyChanged(nameof(Data));
                }
            }
        }
    }
}
