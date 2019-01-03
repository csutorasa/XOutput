using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XOutput.Tools;
using XOutput.UI.Component;

namespace XOutput.UI.Windows
{
    public class MainWindowModel : ModelBase
    {
        private readonly ObservableCollection<InputDeviceView> inputDevices = new ObservableCollection<InputDeviceView>();
        public ObservableCollection<InputDeviceView> InputDevices { get { return inputDevices; } }

        private readonly ObservableCollection<ControllerView> controllers = new ObservableCollection<ControllerView>();
        public ObservableCollection<ControllerView> Controllers { get { return controllers; } }

        private bool allDevices;
        public bool AllDevices
        {
            get => allDevices;
            set
            {
                if (allDevices != value)
                {
                    allDevices = value;
                    Settings.Instance.ShowAllDevices = value;
                    OnPropertyChanged(nameof(AllDevices));
                }
            }
        }

        #region Settings
        public Settings Settings { get; set; }

        private bool settingsOpen;
        public bool SettingsOpen
        {
            get => settingsOpen;
            set
            {
                if (settingsOpen != value)
                {
                    settingsOpen = value;
                    OnPropertyChanged(nameof(SettingsOpen));
                }
            }
        }

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
            get => Settings.CloseToTray;
            set
            {
                if (Settings.CloseToTray != value)
                {
                    Settings.CloseToTray = value;
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
        #endregion
    }
}
