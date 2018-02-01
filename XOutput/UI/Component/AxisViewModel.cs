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
using XOutput.UI.Resources;
using XOutput.UI.View;

namespace XOutput.UI.Component
{
    public class AxisViewModel : ViewModelBase<AxisModel>
    {
        public AxisViewModel(Enum type, int max)
        {
            model = new AxisModel();
            model.Type = type;
            model.Label = type.ToString();
            model.Max = max;
        }
        
        public void updateValues(IDevice device)
        {
            model.Value = (int)(device.Get(model.Type) * model.Max);
        }
    }
}
