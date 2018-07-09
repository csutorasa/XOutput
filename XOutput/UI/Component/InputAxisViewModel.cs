using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices;
using XOutput.Devices.Input;

namespace XOutput.UI.Component
{
    public class InputAxisViewModel : ViewModelBase<InputAxisModel>
    {
        public InputAxisViewModel(InputAxisModel model, Enum type, int max = 1000) : base(model)
        {
            Model.Type = type;
            Model.Max = max;
        }

        public void UpdateValues(IDevice device)
        {
            Model.Value = (int)(device.Get(Model.Type) * Model.Max);
            if (device is IInputDevice)
            {
                Model.RawValue = (int)((device as IInputDevice).GetRaw(Model.Type) * Model.Max);
            }
        }
    }
}
