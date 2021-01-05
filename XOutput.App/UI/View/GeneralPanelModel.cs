using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Core.DependencyInjection;

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

        [ResolverMethod]
        public GeneralPanelModel()
        {

        }
    }
}
