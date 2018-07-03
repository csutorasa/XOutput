using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using XOutput.Devices.Input;

namespace XOutput.UI.Component
{
    public class InputDeviceModel : ModelBase
    {
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

        private string displayName;
        public string DisplayName
        {
            get => displayName;
            set
            {
                if (displayName != value)
                {
                    displayName = value;
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }
    }
}
