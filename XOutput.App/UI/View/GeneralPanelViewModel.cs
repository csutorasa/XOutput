using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using XOutput.App.Configuration;
using XOutput.Configuration;
using XOutput.DependencyInjection;
using XOutput.Rest;
using XOutput.Rest.Help;

namespace XOutput.App.UI.View
{
    public class GeneralPanelViewModel : ViewModelBase<GeneralPanelModel>
    {
        private readonly ConfigurationManager configurationManager;
        private readonly TranslationService translationService;
        private readonly DynamicHttpClientProvider dynamicHttpClientProvider;

        [ResolverMethod]
        public GeneralPanelViewModel(GeneralPanelModel model, ConfigurationManager configurationManager, TranslationService translationService, DynamicHttpClientProvider dynamicHttpClientProvider) : base(model)
        {
            this.configurationManager = configurationManager;
            this.translationService = translationService;
            this.dynamicHttpClientProvider = dynamicHttpClientProvider;
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
            Model.ConnectionErrorMessage = "";
            Model.ServerVersion = "";
            Uri uri;
            try 
            {
                uri = new Uri($"http://{Model.ServerUrl}/api/");
            } 
            catch (UriFormatException)
            {
                Model.ConnectionErrorMessage = "General.InvalidUriConnectionError";
                return;
            }
            try 
            {
                var info = await new InfoClient(new StaticHttpClientProvider(uri)).GetInfoAsync();
                Model.ServerVersion = info.Version;
            } 
            catch
            {
                Model.ConnectionErrorMessage = "General.FailedToConnectToServerError";
                return;
            }
            var appConfig = GetAppConfig();
            appConfig.AutoConnect = Model.AutoConnect;
            appConfig.ServerUrl = Model.ServerUrl;
            configurationManager.Save(appConfig);
            dynamicHttpClientProvider.SetBaseAddress(uri);
        }

        private AppConfig GetAppConfig()
        {
            return configurationManager.Load(() => new AppConfig(translationService.DefaultLanguage));
        }
    }
}
