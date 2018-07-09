using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using XOutput.Devices;
using XOutput.Devices.Input;
using XOutput.Devices.Input.DirectInput;
using XOutput.Devices.XInput;
using XOutput.Tools;
using XOutput.UI.Component;

namespace XOutput.UI.Windows
{
    public class InputDeviceSettingsViewModel : ViewModelBase<InputDeviceSettingsModel>
    {
        private readonly IInputDevice inputDevice;
        private readonly DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private int state = 0;

        public InputDeviceSettingsViewModel(InputDeviceSettingsModel model, IInputDevice inputDevice) : base(model)
        {
            this.inputDevice = inputDevice;
            Model.Title = inputDevice.DisplayName;
            CreateInputControls();
            SetForceFeedback();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += DispatcherTimerTick;
            inputDevice.InputChanged += InputDevice_InputChanged;
            Model.TestButtonText = "Start";
        }

        private void InputDevice_InputChanged(object sender, DeviceInputChangedEventArgs e)
        {
            UpdateInputControls();
        }

        public void Update()
        {
            if (!inputDevice.Connected)
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
                inputDevice.SetForceFeedback(0, 0);
                Model.TestButtonText = "Start";
            }
            else
            {
                dispatcherTimer.Start();
                inputDevice.SetForceFeedback(1, 0);
                Model.TestButtonText = "Stop";
            }
        }

        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            if (state == 0)
            {
                inputDevice.SetForceFeedback(0, 1);
                state = 1;
            }
            else
            {
                inputDevice.SetForceFeedback(1, 0);
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
            foreach (var buttonInput in inputDevice.Buttons)
            {
                Model.InputButtonViews.Add(new ButtonView(new ButtonViewModel(new ButtonModel(), buttonInput)));
            }
            foreach (var sliderInput in inputDevice.Sliders)
            {
                Model.InputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), sliderInput)));
            }
            foreach (var dPadInput in Enumerable.Range(0, inputDevice.DPads.Count()))
            {
                Model.InputDPadViews.Add(new DPadView(new DPadViewModel(new DPadModel(), dPadInput, true)));
            }
        }

        private void UpdateInputControls()
        {
            foreach (var axisView in Model.InputAxisViews)
            {
                axisView.UpdateValues(inputDevice);
            }
            foreach (var buttonView in Model.InputButtonViews)
            {
                buttonView.UpdateValues(inputDevice);
            }
            foreach (var dPadView in Model.InputDPadViews)
            {
                dPadView.UpdateValues(inputDevice);
            }
        }

        private void SetForceFeedback()
        {
            if (inputDevice.ForceFeedbackCount > 0)
            {
                Model.ForceFeedbackEnabled = true;
            }
            else
            {
                Model.ForceFeedbackEnabled = false;
            }
        }

        private void CreateInputAxes()
        {
            var axes = inputDevice.Axes.OfType<DirectInputTypes>();
            var xAxes = DirectInputHelper.Instance.Axes.Where(a => (int)a % 3 == 0);
            var yAxes = DirectInputHelper.Instance.Axes.Where(a => (int)a % 3 == 1);
            var zAxes = DirectInputHelper.Instance.Axes.Where(a => (int)a % 3 == 2);
            for (int i = 0; i < xAxes.Count(); i++)
            {
                var x = xAxes.ElementAt(i);
                var y = yAxes.ElementAt(i);
                if (axes.Contains(x) && axes.Contains(y))
                {
                    Model.InputAxisViews.Add(new Axis2DView(new Axis2DViewModel(new Axis2DModel(), x, y)));
                }
                else
                {
                    if (axes.Contains(x))
                    {
                        Model.InputAxisViews.Add(new InputAxisView(new InputAxisViewModel(new InputAxisModel(Settings.Instance.InputDevices[inputDevice.Id].InputSettings[x]), x)));
                    }
                    if (axes.Contains(y))
                    {
                        Model.InputAxisViews.Add(new InputAxisView(new InputAxisViewModel(new InputAxisModel(Settings.Instance.InputDevices[inputDevice.Id].InputSettings[y]), y)));
                    }
                }
            }
            foreach (var z in zAxes)
            {
                if (axes.Contains(z))
                {
                    Model.InputAxisViews.Add(new InputAxisView(new InputAxisViewModel(new InputAxisModel(Settings.Instance.InputDevices[inputDevice.Id].InputSettings[z]), z)));
                }
            }
        }
    }
}
