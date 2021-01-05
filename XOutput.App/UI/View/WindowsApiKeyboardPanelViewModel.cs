using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.App.Devices.Input.Keyboard;
using XOutput.Core.DependencyInjection;

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
