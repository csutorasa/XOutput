using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using XOutput.Devices;
using XOutput.UI.Windows;

namespace XOutput.UI.Component
{
    public class ControllerViewModel : ViewModelBase<ControllerModel>, IDisposable
    {
        private const int BackgroundDelayMS = 500;
        private readonly DispatcherTimer timer = new DispatcherTimer();

        public ControllerViewModel(ControllerModel model, GameController controller) : base(model)
        {
            Model.Controller = controller;
            Model.ButtonText = "Start";
            Model.Background = Brushes.White;
            timer.Interval = TimeSpan.FromMilliseconds(BackgroundDelayMS);
            timer.Tick += Timer_Tick;
        }

        public void Edit()
        {
            var controllerSettingsWindow = new ControllerSettingsWindow(new ControllerSettingsViewModel(new ControllerSettingsModel(), Model.Controller), Model.Controller);
            controllerSettingsWindow.ShowDialog();
        }

        public void StartStop()
        {
            if (!Model.Started)
            {
                Start();
            }
            else
            {
                Model.Controller.Stop();
            }
        }

        public void Start()
        {
            if (!Model.Started)
            {
                int controllerCount = 0;
                controllerCount = Model.Controller.Start(() =>
                {
                    Model.ButtonText = "Start";
                    Model.Started = false;
                });
                if (controllerCount != 0)
                {
                    Model.ButtonText = "Stop";
                }
                Model.Started = controllerCount != 0;
            }
        }

        public void Dispose()
        {
            timer.Tick -= Timer_Tick;
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
