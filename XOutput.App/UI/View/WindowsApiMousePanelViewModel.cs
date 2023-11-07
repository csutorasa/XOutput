using System.ComponentModel;
using System.Linq;
using System.Windows;
using XOutput.App.Devices.Input;
using XOutput.App.Devices.Input.Mouse;
using XOutput.DependencyInjection;

namespace XOutput.App.UI.View
{
    public class WindowsApiMousePanelViewModel : ViewModelBase<WindowsApiMousePanelModel>
    {
        private readonly MouseDeviceProvider mouseDeviceProvider;

        [ResolverMethod]
        public WindowsApiMousePanelViewModel(WindowsApiMousePanelModel model, MouseDeviceProvider mouseDeviceProvider) : base(model)
        {
            this.mouseDeviceProvider = mouseDeviceProvider;
            Model.Enabled = mouseDeviceProvider.Enabled;
            Model.PropertyChanged += Model_PropertyChanged;
            var mouseDevice = mouseDeviceProvider.GetActiveDevices().First() as MouseDevice;
            mouseDevice.InputChanged += MouseDevice_InputChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.Enabled))
            {
                mouseDeviceProvider.Enabled = Model.Enabled;
            }
        }

        private void MouseDevice_InputChanged(object sender, DeviceInputChangedEventArgs e)
        {
            foreach (var source in e.ChangedValues)
            {
                var mouseButton = (MouseButton) source.Offset;
                bool pressed = source.GetValue() > 0.5;
                switch (mouseButton)
                {
                    case MouseButton.Left:
                        Model.LeftButtonVisibility = pressed ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case MouseButton.Right:
                        Model.RightButtonVisibility = pressed ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case MouseButton.Middle:
                        Model.MiddleButtonVisibility = pressed ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case MouseButton.XButton1:
                        Model.X1ButtonVisibility = pressed ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case MouseButton.XButton2:
                        Model.X2ButtonVisibility = pressed ? Visibility.Visible : Visibility.Collapsed;
                        break;
                }
            }
        }
    }
}
