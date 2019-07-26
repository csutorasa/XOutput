using System.Collections.Generic;
using XOutput.Diagnostics;
using XOutput.UI.Component;

namespace XOutput.UI.Windows
{
    public class DiagnosticsViewModel : ViewModelBase<DiagnosticsModel>
    {
        public DiagnosticsViewModel(DiagnosticsModel model, IEnumerable<IDiagnostics> diagnostics) : base(model)
        {
            foreach (var diagnostic in diagnostics)
            {
                Model.Diagnostics.Add(new DiagnosticsItemView(new DiagnosticsItemViewModel(new DiagnosticsItemModel(), diagnostic)));
            }
        }
    }
}
