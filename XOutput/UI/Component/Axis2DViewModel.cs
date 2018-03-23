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
    public class Axis2DViewModel : ViewModelBase<Axis2DModel>
    {
        public Axis2DViewModel(Axis2DModel model, Enum typex, Enum typey, int maxx = 42, int maxy = 42) : base(model)
        {
            Model.TypeX = typex;
            Model.MaxX = maxx;
            Model.TypeY = typey;
            Model.MaxY = maxy;
            Model.TwoD = true;
        }

        public void UpdateValues(IDevice device)
        {
            Model.ValueX = (int)(device.Get(Model.TypeX) * Model.MaxX);
            Model.ValueY = (int)(Model.MaxY - device.Get(Model.TypeY) * Model.MaxY);
        }
    }
}
