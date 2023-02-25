using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;
using XOutput.Devices;
using XOutput.UI.Windows;

namespace XOutput.UI.Component
{
    public class ControllerViewModel : ViewModelBase<ControllerModel>, IDisposable
    {
        private const int BackgroundDelayMS = 500;
        private readonly Action<string> log;
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly bool isAdmin;

        public ControllerViewModel(ControllerModel model, GameController controller, bool isAdmin, Action<string> log) : base(model)
        {
            this.log = log;
            this.isAdmin = isAdmin;
            Model.Controller = controller;
            Model.ButtonText = "Start";
            Model.Background = Brushes.White;
            Model.Controller.XInput.InputChanged += InputDevice_InputChanged;
            Model.SelectedOutputIndex = OutputDevices.Instance.GetDevices().IndexOf(Model.Controller.XOutputInterface);
            timer.Interval = TimeSpan.FromMilliseconds(BackgroundDelayMS);
            timer.Tick += Timer_Tick;
            
            outputGroupItems = new List<string>(OutputDevices.MaxOutputDevices);
            for (var i = 1; i <= OutputDevices.MaxOutputDevices; i++)
            {
                outputGroupItems.Add($"Controller {i}");
            }
        }
        
        public void Edit()
        {
            var controllerSettingsWindow = new ControllerSettingsWindow(new ControllerSettingsViewModel(new ControllerSettingsModel(), Model.Controller, isAdmin), Model.Controller);
            controllerSettingsWindow.ShowDialog();
            Model.RefreshName();
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
                int controllerCount = -1;
                controllerCount = Model.Controller.Start(() =>
                {
                    Model.ButtonText = "Start";
                    log?.Invoke(string.Format(LanguageModel.Instance.Translate("EmulationStopped"), Model.Controller.DisplayName));
                    Model.Started = false;
                });
                if (controllerCount > -1)
                {
                    Model.ButtonText = "Stop";
                    log?.Invoke(string.Format(LanguageModel.Instance.Translate("EmulationStarted"), Model.Controller.DisplayName, controllerCount));
                }
                Model.Started = controllerCount > -1;
            }
        }

        public void Dispose()
        {
            timer.Tick -= Timer_Tick;
            Model.Controller.XInput.InputChanged -= InputDevice_InputChanged;
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

        public void SetOutputDevice(int selectedIndex)
        {
            var outputDevices = OutputDevices.Instance.GetDevices();
            Model.Controller.XOutputInterface =
                outputDevices.ElementAtOrDefault(selectedIndex) ?? outputDevices.First();
            Model.Controller.Mapper.OutputDeviceIndex = selectedIndex;
            Model.SelectedOutputIndex = selectedIndex;
        }
        
        private readonly List<string> outputGroupItems;
        public List<string> OutputGroupItems => outputGroupItems;
    }
}
