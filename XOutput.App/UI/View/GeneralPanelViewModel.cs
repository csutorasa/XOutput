using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI.View
{
    public class GeneralPanelViewModel : ViewModelBase<GeneralPanelModel>
    {
        private readonly TranslationService translationService;

        [ResolverMethod]
        public GeneralPanelViewModel(GeneralPanelModel model, TranslationService translationService) : base(model)
        {
            this.translationService = translationService;
            foreach (var language in translationService.GetAvailableLanguages())
            {
                Model.Languages.Add(language);
            }
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.SelectedLanguage))
            {
                translationService.Load(Model.SelectedLanguage);
            }
        }
    }
}
