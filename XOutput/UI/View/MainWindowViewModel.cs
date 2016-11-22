using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.UI.Component;

namespace XOutput.UI.View
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ObservableCollection<ControllerView> controllers = new ObservableCollection<ControllerView>();
        public ObservableCollection<ControllerView> Controllers { get { return controllers; } }
    }
}
