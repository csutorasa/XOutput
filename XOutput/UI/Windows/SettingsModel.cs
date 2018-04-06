using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Tools;
using XOutput.UI.Component;

namespace XOutput.UI.Windows
{
    public class SettingsModel : ModelBase
    {
        private readonly Settings settings;

        private readonly ObservableCollection<string> languages = new ObservableCollection<string>();
        public ObservableCollection<string> Languages => languages;

        private string selectedLanguage;
        public string SelectedLanguage
        {
            get => LanguageManager.Instance.Language;
            set
            {
                if (selectedLanguage != value)
                {
                    selectedLanguage = value;
                    LanguageManager.Instance.Language = value;
                    OnPropertyChanged(nameof(SelectedLanguage));
                }
            }
        }

        public bool CloseToTray
        {
            get => settings.CloseToTray;
            set
            {
                if (settings.CloseToTray != value)
                {
                    settings.CloseToTray = value;
                    OnPropertyChanged(nameof(CloseToTray));
                }
            }
        }

        public bool RunAtStartup
        {
            get => RegistryModifier.Instance.Autostart;
            set
            {
                if (RegistryModifier.Instance.Autostart != value)
                {
                    RegistryModifier.Instance.Autostart = value;
                    OnPropertyChanged(nameof(RunAtStartup));
                }
            }
        }

        public SettingsModel(Settings settings)
        {
            this.settings = settings;
        }
    }
}
