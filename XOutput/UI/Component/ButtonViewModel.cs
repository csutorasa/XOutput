using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices;

namespace XOutput.UI.Component
{
    public class ButtonViewModel : ViewModelBase<ButtonModel>
    {
        public ButtonViewModel(ButtonModel model, InputSource type) : base(model)
        {
            Model.Type = type;
        }

        public void UpdateValues(IDevice device)
        {
            Model.Value = device.Get(Model.Type) > 0.5;
        }
    }
}
