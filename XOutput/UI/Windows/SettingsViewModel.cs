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
            foreach (var language in LanguageManager.Instance.GetLanguages())
            {
                Model.Languages.Add(language);
            }
        }
    }
}
