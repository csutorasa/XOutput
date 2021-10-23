using System.ComponentModel;
using XOutput.App.Devices.Input.Keyboard;
using XOutput.DependencyInjection;

namespace XOutput.App.UI.View
{
    public class WindowsApiKeyboardPanelViewModel : ViewModelBase<WindowsApiKeyboardPanelModel>
    {
        private readonly KeyboardDeviceProvider keyboardDeviceProvider;

        [ResolverMethod]
        public WindowsApiKeyboardPanelViewModel(WindowsApiKeyboardPanelModel model, KeyboardDeviceProvider keyboardDeviceProvider) : base(model)
        {
            this.keyboardDeviceProvider = keyboardDeviceProvider;
            Model.Enabled = keyboardDeviceProvider.Enabled;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.Enabled))
            {
                keyboardDeviceProvider.Enabled = Model.Enabled;
            }
        }
    }
}
