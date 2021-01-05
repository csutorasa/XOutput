using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI
{
    public class TranslationModel : ModelBase
    {
        public static TranslationModel Instance => instance;
        private static TranslationModel instance = new TranslationModel();

        private string language;
        public string Language
        {
            get => language;
            set
            {
                if (language != value)
                {
                    language = value;
                    OnPropertyChanged(nameof(Language));
                }
            }
        }

    }
}
