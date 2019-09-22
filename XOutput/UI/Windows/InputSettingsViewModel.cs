using System;
using System.Linq;
using System.Windows.Threading;
using XOutput.Devices;
using XOutput.Devices.Input;
using XOutput.Devices.XInput.Vigem;
using XOutput.Tools;
using XOutput.UI.Component;

namespace XOutput.UI.Windows
{
    public class InputSettingsViewModel : ViewModelBase<InputSettingsModel>, IDisposable
    {
        private readonly HidGuardianManager hidGuardianManager;
        private readonly IInputDevice device;
        private readonly DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private int state = 0;

        public InputSettingsViewModel(InputSettingsModel model, HidGuardianManager hidGuardianManager, IInputDevice device, bool isAdmin) : base(model)
        {
            this.hidGuardianManager = hidGuardianManager;
            this.device = device;
            Model.IsAdmin = isAdmin && device.HardwareID != null;
            if (Model.IsAdmin)
            {
                Model.HidGuardianAdded = hidGuardianManager.IsAffected(device.HardwareID);
            }
            Model.Title = device.DisplayName;
            CreateInputControls();
            SetForceFeedback();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += DispatcherTimerTick;
            Model.TestButtonText = "Start";
            Model.ForceFeedbackEnabled = device.InputConfiguration.ForceFeedback;
        }

        public void Update()
        {
            if (!device.Connected)
            {
                return;
            }

            UpdateInputControls();
        }

        public void TestForceFeedback()
        {
            if (dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Stop();
                device.SetForceFeedback(0, 0);
                Model.TestButtonText = "Start";
            }
            else
            {
                dispatcherTimer.Start();
                device.SetForceFeedback(1, 0);
                Model.TestButtonText = "Stop";
            }
        }

        public void SetForceFeedbackEnabled()
        {
            device.InputConfiguration.ForceFeedback = Model.ForceFeedbackEnabled;
        }

        public void AddHidGuardian()
        {
            hidGuardianManager.AddAffectedDevice(device.HardwareID);
            if (Model.IsAdmin)
            {
                Model.HidGuardianAdded = hidGuardianManager.IsAffected(device.HardwareID);
            }
        }

        public void RemoveHidGuardian()
        {
            hidGuardianManager.RemoveAffectedDevice(device.HardwareID);
            if (Model.IsAdmin)
            {
                Model.HidGuardianAdded = hidGuardianManager.IsAffected(device.HardwareID);
            }
        }

        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            if (state == 0)
            {
                device.SetForceFeedback(0, 1);
                state = 1;
            }
            else
            {
                device.SetForceFeedback(1, 0);
                state = 0;
            }
        }

        public void Dispose()
        {
            Model.InputAxisViews.Clear();
            Model.InputButtonViews.Clear();
            Model.InputDPadViews.Clear();
        }

        private void CreateInputControls()
        {
            CreateInputAxes();
            foreach (var buttonInput in device.Sources.Where(s => s.Type == InputSourceTypes.Button))
            {
                Model.InputButtonViews.Add(new ButtonView(new ButtonViewModel(new ButtonModel(), buttonInput)));
            }
            foreach (var sliderInput in device.Sources.Where(s => s.Type == InputSourceTypes.Slider))
            {
                Model.InputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), sliderInput)));
            }
            foreach (var dPadInput in Enumerable.Range(0, device.DPads.Count()))
            {
                Model.InputDPadViews.Add(new DPadView(new DPadViewModel(new DPadModel(), dPadInput, true)));
            }
        }

        private void UpdateInputControls()
        {
            foreach (var axisView in Model.InputAxisViews)
            {
                axisView.UpdateValues(device);
            }
            foreach (var buttonView in Model.InputButtonViews)
            {
                buttonView.UpdateValues(device);
            }
            foreach (var dPadView in Model.InputDPadViews)
            {
                dPadView.UpdateValues(device);
            }
        }

        private void SetForceFeedback()
        {
            if (device.ForceFeedbackCount > 0)
            {
                if (VigemDevice.IsAvailable())
                {
                    Model.ForceFeedbackText = "";
                    Model.ForceFeedbackEnabled = device.InputConfiguration.ForceFeedback;
                    Model.ForceFeedbackAvailable = true;
                }
                else
                {
                    Model.ForceFeedbackEnabled = false;
                    Model.ForceFeedbackAvailable = false;
                    Model.ForceFeedbackText = "ForceFeedbackVigemOnly";
                }
            }
            else
            {
                Model.ForceFeedbackEnabled = false;
                Model.ForceFeedbackAvailable = false;
                Model.ForceFeedbackText = "ForceFeedbackUnsupported";
            }
        }

        private void CreateInputAxes()
        {
            var axes = device.Sources.Where(s => InputSourceTypes.Axis.HasFlag(s.Type)).ToArray();
            foreach (var z in axes)
            {
                if (axes.Contains(z))
                {
                    Model.InputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), z)));
                }
            }
        }
    }
}
