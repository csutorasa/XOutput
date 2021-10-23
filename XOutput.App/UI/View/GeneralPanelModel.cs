using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.DependencyInjection;

namespace XOutput.App.UI.View
{
    public class GeneralPanelModel : ModelBase
    {
        private readonly ObservableCollection<string> languages = new ObservableCollection<string>();
        public ObservableCollection<string> Languages => languages;

        private string selectedLanguage;
        public string SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                if (selectedLanguage != value)
                {
                    selectedLanguage = value;
                    OnPropertyChanged(nameof(SelectedLanguage));
                }
            }
        }

        private string serverUrl;
        public string ServerUrl
        {
            get => serverUrl;
            set
            {
                if (serverUrl != value)
                {
                    serverUrl = value;
                    OnPropertyChanged(nameof(ServerUrl));
                }
            }
        }

        private bool autoConnect;
        public bool AutoConnect
        {
            get => autoConnect;
            set
            {
                if (autoConnect != value)
                {
                    autoConnect = value;
                    OnPropertyChanged(nameof(AutoConnect));
                }
            }
        }

        [ResolverMethod]
        public GeneralPanelModel()
        {

        }
    }
}
