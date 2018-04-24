using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using XOutput.Tools;

namespace XOutput.UI.Windows
{
    public class SettingsViewModel : ViewModelBase<SettingsModel>
    {
        public SettingsViewModel(SettingsModel model) : base(model)
        {
            var languages = LanguageManager.Instance.GetLanguages().ToList();
            languages.Sort();
            foreach (var language in languages)
            {
                Model.Languages.Add(language);
            }
        }
    }
}
