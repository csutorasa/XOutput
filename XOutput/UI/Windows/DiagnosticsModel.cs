using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XOutput.Diagnostics;
using XOutput.UI.Component;

namespace XOutput.UI.Windows
{
    public class DiagnosticsModel : ModelBase
    {
        private readonly ObservableCollection<DiagnosticsItemView> diagnostics = new ObservableCollection<DiagnosticsItemView>();
        public ObservableCollection<DiagnosticsItemView> Diagnostics => diagnostics;
    }
}
