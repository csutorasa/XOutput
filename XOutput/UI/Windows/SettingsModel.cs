using System.Collections.ObjectModel;
using XOutput.Tools;

namespace XOutput.UI.Windows
{
    public class SettingsModel : ModelBase
    {
        private readonly Settings settings;
        private readonly RegistryModifier registryModifier;

        private readonly ObservableCollection<string> languages = new ObservableCollection<string>();
        public ObservableCollection<string> Languages => languages;

        private string selectedLanguage;
        public string SelectedLanguage
        {
            get
            {
                selectedLanguage = LanguageManager.Instance.Language;
                return selectedLanguage;
            }
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
            get => registryModifier.Autostart;
            set
            {
                if (registryModifier.Autostart != value)
                {
                    registryModifier.Autostart = value;
                    OnPropertyChanged(nameof(RunAtStartup));
                }
            }
        }

        public bool HidGuardianEnabled
        {
            get => settings.HidGuardianEnabled;
            set
            {
                if (settings.HidGuardianEnabled != value)
                {
                    settings.HidGuardianEnabled = value;
                    OnPropertyChanged(nameof(HidGuardianEnabled));
                }
            }
        }

        public bool DisableAutoRefresh
        {
            get => settings.DisableAutoRefresh;
            set
            {
                if (settings.DisableAutoRefresh != value)
                {
                    settings.DisableAutoRefresh = value;
                    OnPropertyChanged(nameof(DisableAutoRefresh));
                }
            }
        }

        public SettingsModel(RegistryModifier registryModifier, Settings settings)
        {
            this.registryModifier = registryModifier;
            this.settings = settings;
        }
    }
}
