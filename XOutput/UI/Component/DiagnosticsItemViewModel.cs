using XOutput.Devices.Input;
using XOutput.Diagnostics;

namespace XOutput.UI.Component
{
    public class DiagnosticsItemViewModel : ViewModelBase<DiagnosticsItemModel>
    {
        public DiagnosticsItemViewModel(DiagnosticsItemModel model, IDiagnostics diagnostics) : base(model)
        {
            Model.Source = SourceToString(diagnostics.Source);
            foreach (var result in diagnostics.GetResults())
            {
                Model.Results.Add(result);
            }
        }

        protected string SourceToString(object source)
        {
            if (source == null)
            {
                return LanguageModel.Instance.Translate("System");
            }

            if (source is IInputDevice)
            {
                return (source as IInputDevice).DisplayName;
            }
            return null;
        }
    }
}
