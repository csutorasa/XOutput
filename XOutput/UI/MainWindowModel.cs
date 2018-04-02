using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input;
using XOutput.UI.Component;

namespace XOutput.UI
{
    public class MainWindowModel : ModelBase
    {
        private readonly ObservableCollection<ControllerView> controllers = new ObservableCollection<ControllerView>();
        public ObservableCollection<ControllerView> Controllers { get { return controllers; } }

        private bool allDevices;
        public bool AllDevices
        {
            get => allDevices;
            set
            {
                if (allDevices != value)
                {
                    allDevices = value;
                    OnPropertyChanged(nameof(AllDevices));
                }
            }
        }
    }
}
