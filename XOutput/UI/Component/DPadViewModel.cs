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
    public class DPadViewModel : ViewModelBase<DPadModel>
    {
        private const int len = 21;

        public DPadViewModel()
        {
            model = new DPadModel();
        }

        public void UpdateValues(IDevice device)
        {
            model.Direction = device.DPad;
            if (model.Direction.HasFlag(DPadDirection.Up))
            {
                model.ValueY = -len;
            }
            else if (model.Direction.HasFlag(DPadDirection.Down))
            {
                model.ValueY = len;
            }
            else
            {
                model.ValueY = 0;
            }
            model.ValueY += 21;
            if (model.Direction.HasFlag(DPadDirection.Right))
            {
                model.ValueX = len;
            }
            else if (model.Direction.HasFlag(DPadDirection.Left))
            {
                model.ValueX = -len;
            }
            else
            {
                model.ValueX = 0;
            }
            model.ValueX += 21;

            model.Direction = device.DPad;
        }
    }
}
