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
        public Axis2DViewModel(Enum typex, Enum typey, int maxx, int maxy)
        {
            model = new Axis2DModel();
            model.TypeX = typex;
            model.MaxX = maxx;
            model.TypeY = typey;
            model.MaxY = maxy;
            model.TwoD = true;
        }

        public void UpdateValues(IDevice device)
        {
            model.ValueX = (int)(device.Get(model.TypeX) * model.MaxX);
            model.ValueY = (int)(model.MaxY - device.Get(model.TypeY) * model.MaxY);
        }
    }
}
