using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.App.Configuration;
using XOutput.Client.Help;
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
            var appConfig = GetAppConfig();
            Model.AutoConnect = appConfig.AutoConnect;
            Model.ServerUrl = appConfig.ServerUrl;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.SelectedLanguage))
            {
                if (translationService.Load(Model.SelectedLanguage))
                {
                    var appConfig = GetAppConfig();
                    appConfig.Language = Model.SelectedLanguage;
                    configurationManager.Save(appConfig);
                }
                
            }
        }

        public async Task Connect()
        {
            var x = await new InfoClient(new Uri($"http://{Model.ServerUrl}/api/")).GetInfoAsync();
            var appConfig = GetAppConfig();
            appConfig.AutoConnect = Model.AutoConnect;
            appConfig.ServerUrl = Model.ServerUrl;
            configurationManager.Save(appConfig);
        }

        private AppConfig GetAppConfig()
        {
            return configurationManager.Load(() => new AppConfig(translationService.DefaultLanguage));
        }
    }
}
