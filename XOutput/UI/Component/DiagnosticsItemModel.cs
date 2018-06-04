using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XOutput.Devices.XInput;
using XOutput.Diagnostics;

namespace XOutput.UI
{
    public class DiagnosticsItemModel : ModelBase
    {
        private readonly ObservableCollection<DiagnosticsResult> results = new ObservableCollection<DiagnosticsResult>();
        public ObservableCollection<DiagnosticsResult> Results => results;

        private string source;
        public string Source
        {
            get => source;
            set
            {
                if (source != value)
                {
                    source = value;
                    OnPropertyChanged(nameof(Source));
                }
            }
        }
    }
}
