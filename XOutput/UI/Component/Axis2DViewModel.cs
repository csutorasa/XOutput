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
    public class Axis2DViewModel : ViewModelBase<Axis2DModel>
    {
        public Axis2DViewModel(Enum typex, Enum typey, int maxx, int maxy)
        {
            model = new Axis2DModel();
            model.TypeX = typex;
            model.LabelX = typex.ToString();
            model.MaxX = maxx;
            model.TypeY = typey;
            model.LabelY = typey.ToString();
            model.MaxY = maxy;
        }

        public void updateValues(IDevice device)
        {
            model.ValueX = (int)(device.Get(model.TypeX) * model.MaxX);
            model.ValueY = (int)(device.Get(model.TypeY) * model.MaxY);
        }
    }
}
