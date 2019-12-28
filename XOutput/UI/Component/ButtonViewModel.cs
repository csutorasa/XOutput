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
            Model.Value = Model.Type.Value > 0.5;
        }
    }
}
