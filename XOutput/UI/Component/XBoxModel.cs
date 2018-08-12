using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using XOutput.Devices;
using XOutput.Devices.XInput;

namespace XOutput.UI
{
    public class XBoxModel : ModelBase
    {
        private InputType xInputType;
        public InputType XInputType
        {
            get => xInputType;
            set
            {
                if (xInputType != value)
                {
                    xInputType = value;
                    OnPropertyChanged(nameof(XInputType));
                }
            }
        }

        private bool highlight;
        public bool Highlight
        {
            get => highlight;
            set
            {
                if (highlight != value)
                {
                    highlight = value;
                    OnPropertyChanged(nameof(Highlight));
                }
            }
        }
    }
}
