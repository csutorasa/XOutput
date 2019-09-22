using System;
using System.Windows.Media;
using System.Windows.Threading;
using XOutput.Devices;
using XOutput.Devices.Input;
using XOutput.Tools;
using XOutput.UI.Windows;

namespace XOutput.UI.Component
{
    public class InputViewModel : ViewModelBase<InputModel>, IDisposable
    {
        private const int BackgroundDelayMS = 500;
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly bool isAdmin;

        public InputViewModel(InputModel model, IInputDevice device, bool isAdmin) : base(model)
        {
            this.isAdmin = isAdmin;
            Model.Device = device;
            Model.Background = Brushes.White;
            Model.Device.InputChanged += InputDevice_InputChanged;
            timer.Interval = TimeSpan.FromMilliseconds(BackgroundDelayMS);
            timer.Tick += Timer_Tick;
        }

        public void Edit()
        {
            HidGuardianManager hidGuardianManager = ApplicationContext.Global.Resolve<HidGuardianManager>();
            var controllerSettingsWindow = new InputSettingsWindow(new InputSettingsViewModel(new InputSettingsModel(), hidGuardianManager, Model.Device, isAdmin), Model.Device);
            controllerSettingsWindow.ShowDialog();
        }

        public void Dispose()
        {
            timer.Tick -= Timer_Tick;
            Model.Device.InputChanged -= InputDevice_InputChanged;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Model.Background = Brushes.White;
        }

        private void InputDevice_InputChanged(object sender, DeviceInputChangedEventArgs e)
        {
            Model.Background = Brushes.LightGreen;
            timer.Stop();
            timer.Start();
        }
    }
}
