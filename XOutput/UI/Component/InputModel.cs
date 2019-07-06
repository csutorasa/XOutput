using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XOutput.Devices;
using XOutput.Devices.Input;

namespace XOutput.UI.Component
{
    public class InputModel : ModelBase
    {
        private IInputDevice device;
        public IInputDevice Device
        {
            get => device;
            set
            {
                if (device != value)
                {
                    device = value;
                    OnPropertyChanged(nameof(Device));
                }
            }
        }

        private Brush background;
        public Brush Background
        {
            get => background;
            set
            {
                if (background != value)
                {
                    background = value;
                    OnPropertyChanged(nameof(Background));
                }
            }
        }

        public string DisplayName { get { return string.Format("{0} ({1})", device.DisplayName, device.UniqueId); } }
    }
}
