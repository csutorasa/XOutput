using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using XOutput.Devices;
using XOutput.Devices.Input;
using XOutput.UI.Windows;

namespace XOutput.UI.Component
{
    public class InputDeviceViewModel : ViewModelBase<InputDeviceModel>, IDisposable
    {
        public InputDeviceViewModel(InputDeviceModel model, IInputDevice inputDevice) : base(model)
        {
            Model.InputDevice = inputDevice;
        }

        public void Dispose()
        {

        }
    }
}
