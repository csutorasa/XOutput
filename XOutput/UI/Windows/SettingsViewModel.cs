using System.Linq;
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
