using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.App.Configuration;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI.View
{
    public class GeneralPanelViewModel : ViewModelBase<GeneralPanelModel>
    {
        private readonly ConfigurationManager configurationManager;
        private readonly TranslationService translationService;

        [ResolverMethod]
        public GeneralPanelViewModel(GeneralPanelModel model, ConfigurationManager configurationManager, TranslationService translationService) : base(model)
        {
            this.configurationManager = configurationManager;
            this.translationService = translationService;
            foreach (var language in translationService.GetAvailableLanguages())
            {
                Model.Languages.Add(language);
            }
            Model.SelectedLanguage = TranslationModel.Instance.Language;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.SelectedLanguage))
            {
                if (translationService.Load(Model.SelectedLanguage))
                {
                    var appConfig = configurationManager.Load(() => new AppConfig(translationService.DefaultLanguage));
                    appConfig.Language = Model.SelectedLanguage;
                    configurationManager.Save(appConfig);
                }
                
            }
        }
    }
}
