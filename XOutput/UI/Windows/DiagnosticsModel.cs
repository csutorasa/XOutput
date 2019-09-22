using System.Collections.Generic;
using System.Collections.ObjectModel;
using XOutput.Diagnostics;
using XOutput.UI.Component;

namespace XOutput.UI.Windows
{
    public class DiagnosticsModel : ModelBase
    {
        private readonly ObservableCollection<DiagnosticsItemView> diagnostics = new ObservableCollection<DiagnosticsItemView>();
        public ObservableCollection<DiagnosticsItemView> Diagnostics => diagnostics;

        public DiagnosticsModel(IEnumerable<IDiagnostics> diagnostics)
        {
            foreach (var diagnostic in diagnostics)
            {
                Diagnostics.Add(new DiagnosticsItemView(new DiagnosticsItemViewModel(new DiagnosticsItemModel(), diagnostic)));
            }
        }
    }
}
