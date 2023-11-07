using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using XOutput.App.Devices.Input;
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
            var keyboardDevice = keyboardDeviceProvider.GetActiveDevices().First() as KeyboardDevice;
            keyboardDevice.InputChanged += KeyboardDevice_InputChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.Enabled))
            {
                keyboardDeviceProvider.Enabled = Model.Enabled;
            }
        }
        private void KeyboardDevice_InputChanged(object sender, DeviceInputChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => {
                foreach (var source in e.ChangedValues)
                {
                    var keyboardButton = (KeyboardButton) source.Offset;
                    bool pressed = source.GetValue() > 0.5;
                    if (pressed && !Model.PressedButtons.Contains(keyboardButton))
                    {
                        Model.PressedButtons.Add(keyboardButton);
                    }
                    if (!pressed && Model.PressedButtons.Contains(keyboardButton))
                    {
                        Model.PressedButtons.Remove(keyboardButton);
                    }
                }
            });
        }
    }
}
