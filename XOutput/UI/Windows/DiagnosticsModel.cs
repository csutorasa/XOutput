using System.Collections.ObjectModel;
using XOutput.UI.Component;

namespace XOutput.UI.Windows
{
    public class DiagnosticsModel : ModelBase
    {
        private readonly ObservableCollection<DiagnosticsItemView> diagnostics = new ObservableCollection<DiagnosticsItemView>();
        public ObservableCollection<DiagnosticsItemView> Diagnostics => diagnostics;
    }
}
