using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.Input.XInput;
using XOutput.UI.View;

namespace XOutput.UI.Component
{
    public class ButtonViewModel : ViewModelBase<ButtonModel>
    {
        public ButtonViewModel(Enum type)
        {
            model = new ButtonModel();
            model.Type = type;
        }

        public void UpdateValues(IDevice device)
        {
            Model.Value = device.Get(Model.Type) > 0.5;
        }
    }
}
