using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices;

namespace XOutput.UI.Component
{
    public class AxisViewModel : ViewModelBase<AxisModel>
    {
        public AxisViewModel(AxisModel model, InputSource type, int max = 1000) : base(model)
        {
            Model.Type = type;
            Model.Max = max;
        }

        public void UpdateValues(IDevice device)
        {
            Model.Value = (int)(device.Get(Model.Type) * Model.Max);
        }
    }
}
