using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XOutput.Tools;
using XOutput.UI.Component;

namespace XOutput.UI.Windows
{
    public class MainWindowModel : ModelBase
    {
        private readonly ObservableCollection<InputDeviceView> inputDevices = new ObservableCollection<InputDeviceView>();
        public ObservableCollection<InputDeviceView> InputDevices { get { return inputDevices; } }

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
                    Settings.Instance.ShowAllDevices = value;
                    OnPropertyChanged(nameof(AllDevices));
                }
            }
        }
    }
}
