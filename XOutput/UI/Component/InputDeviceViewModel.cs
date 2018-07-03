using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using XOutput.Devices;
using XOutput.Devices.Input;
using XOutput.UI.Windows;

namespace XOutput.UI.Component
{
    public class InputDeviceViewModel : ViewModelBase<InputDeviceModel>, IDisposable
    {
        private const int BackgroundDelayMS = 500;
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly IInputDevice inputDevice;
        public IInputDevice InputDevice => inputDevice;

        public InputDeviceViewModel(InputDeviceModel model, IInputDevice inputDevice) : base(model)
        {
            this.inputDevice = inputDevice;
            Model.Background = Brushes.White;
            Model.DisplayName = inputDevice.ToString();
            inputDevice.InputChanged += InputDevice_InputChanged;
            timer.Interval = TimeSpan.FromMilliseconds(BackgroundDelayMS);
            timer.Tick += Timer_Tick;
        }

        public void Dispose()
        {
            inputDevice.InputChanged -= InputDevice_InputChanged;
            timer.Tick -= Timer_Tick;
        }

        public void OpenSettings()
        {
            new InputDeviceSettingsWindow(new InputDeviceSettingsViewModel(new InputDeviceSettingsModel(), inputDevice)).ShowDialog();
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
