using System.Linq;
using XOutput.Devices;

namespace XOutput.UI.Component
{
    public class DPadViewModel : ViewModelBase<DPadModel>
    {
        private const int len = 21;
        private readonly int dPadIndex;

        public DPadViewModel(DPadModel model, int dPadIndex, bool showLabel) : base(model)
        {
            this.dPadIndex = dPadIndex;
            if (showLabel)
            {
                Model.Label = "DPad" + (dPadIndex + 1);
            }
        }

        public void UpdateValues(IDevice device)
        {
            Model.Direction = device.DPads.ElementAt(dPadIndex);
            if (Model.Direction.HasFlag(DPadDirection.Up))
            {
                Model.ValueY = -len;
            }
            else if (Model.Direction.HasFlag(DPadDirection.Down))
            {
                Model.ValueY = len;
            }
            else
            {
                Model.ValueY = 0;
            }
            Model.ValueY += 21;
            if (Model.Direction.HasFlag(DPadDirection.Right))
            {
                Model.ValueX = len;
            }
            else if (Model.Direction.HasFlag(DPadDirection.Left))
            {
                Model.ValueX = -len;
            }
            else
            {
                Model.ValueX = 0;
            }
            Model.ValueX += 21;
        }
    }
}
