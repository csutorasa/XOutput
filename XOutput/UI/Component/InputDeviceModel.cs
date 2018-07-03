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
        private IInputDevice inputDevice;
        public IInputDevice InputDevice
        {
            get => inputDevice;
            set
            {
                if (inputDevice != value)
                {
                    inputDevice = value;
                    OnPropertyChanged(nameof(InputDevice));
                }
            }
        }

        public string DisplayName { get { return InputDevice.ToString(); } }
    }
}
