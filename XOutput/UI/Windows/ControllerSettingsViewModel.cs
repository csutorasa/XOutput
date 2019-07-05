using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using XOutput.Devices;
using XOutput.Devices.Input.DirectInput;
using XOutput.Devices.XInput;
using XOutput.Tools;
using XOutput.UI.Component;

namespace XOutput.UI.Windows
{
    public class ControllerSettingsViewModel : ViewModelBase<ControllerSettingsModel>
    {
        private readonly GameController controller;
        private readonly DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private int state = 0;

        public ControllerSettingsViewModel(ControllerSettingsModel model, GameController controller, bool isAdmin) : base(model)
        {
            this.controller = controller;
            Model.IsAdmin = isAdmin && controller.InputDevice.HardwareID != null;
            if (Model.IsAdmin)
            {
                Model.HidGuardianAdded = HidGuardianManager.Instance.IsAffected(controller.InputDevice.HardwareID);
            }
            Model.Title = controller.DisplayName;
            CreateInputControls();
            CreateMappingControls();
            CreateXInputControls();
            SetForceFeedback();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += DispatcherTimerTick;
            Model.TestButtonText = "Start";
            Model.ForceFeedbackEnabled = controller.InputDevice.InputConfiguration.ForceFeedback;
            Model.StartWhenConnected = controller.InputDevice.InputConfiguration.StartWhenConnected;
        }

        public void ConfigureAll()
        {
            var types = XInputHelper.Instance.Values;
            if (controller.InputDevice.DPads.Any())
            {
                types = types.Where(t => !t.IsDPad());
            }
            new AutoConfigureWindow(new AutoConfigureViewModel(new AutoConfigureModel(), controller, types.ToArray()), types.Any()).ShowDialog();
            foreach (var v in Model.MapperAxisViews.Concat(Model.MapperButtonViews).Concat(Model.MapperDPadViews))
            {
                v.Refresh();
            }
        }

        public void Update()
        {
            if (!controller.InputDevice.Connected)
            {
                return;
            }

            UpdateInputControls();

            UpdateXInputControls();
        }

        public void TestForceFeedback()
        {
            if (dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Stop();
                controller.InputDevice.SetForceFeedback(0, 0);
                Model.TestButtonText = "Start";
            }
            else
            {
                dispatcherTimer.Start();
                controller.InputDevice.SetForceFeedback(1, 0);
                Model.TestButtonText = "Stop";
            }
        }

        public void SetStartWhenConnected()
        {
            controller.InputDevice.InputConfiguration.StartWhenConnected = Model.StartWhenConnected;
        }

        public void SetForceFeedbackEnabled()
        {
            controller.InputDevice.InputConfiguration.ForceFeedback = Model.ForceFeedbackEnabled;
        }

        public void AddHidGuardian()
        {
            HidGuardianManager.Instance.AddAffectedDevice(controller.InputDevice.HardwareID);
            if (Model.IsAdmin)
            {
                Model.HidGuardianAdded = HidGuardianManager.Instance.IsAffected(controller.InputDevice.HardwareID);
            }
        }

        public void RemoveHidGuardian()
        {
            HidGuardianManager.Instance.RemoveAffectedDevice(controller.InputDevice.HardwareID);
            if (Model.IsAdmin)
            {
                Model.HidGuardianAdded = HidGuardianManager.Instance.IsAffected(controller.InputDevice.HardwareID);
            }
        }

        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            if (state == 0)
            {
                controller.InputDevice.SetForceFeedback(0, 1);
                state = 1;
            }
            else
            {
                controller.InputDevice.SetForceFeedback(1, 0);
                state = 0;
            }
        }

        public void Dispose()
        {
            Model.InputAxisViews.Clear();
            Model.InputButtonViews.Clear();
            Model.InputDPadViews.Clear();
            Model.XInputAxisViews.Clear();
            Model.XInputButtonViews.Clear();
            Model.XInputDPadViews.Clear();
            Model.MapperAxisViews.Clear();
            Model.MapperButtonViews.Clear();
            Model.MapperDPadViews.Clear();
        }

        private void CreateInputControls()
        {
            CreateInputAxes();
            foreach (var buttonInput in controller.InputDevice.Sources.Where(s => s.Type == InputSourceTypes.Button))
            {
                Model.InputButtonViews.Add(new ButtonView(new ButtonViewModel(new ButtonModel(), buttonInput)));
            }
            foreach (var sliderInput in controller.InputDevice.Sources.Where(s => s.Type == InputSourceTypes.Slider))
            {
                Model.InputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), sliderInput)));
            }
            foreach (var dPadInput in Enumerable.Range(0, controller.InputDevice.DPads.Count()))
            {
                Model.InputDPadViews.Add(new DPadView(new DPadViewModel(new DPadModel(), dPadInput, true)));
            }
        }

        private void UpdateInputControls()
        {
            foreach (var axisView in Model.InputAxisViews)
            {
                axisView.UpdateValues(controller.InputDevice);
            }
            foreach (var buttonView in Model.InputButtonViews)
            {
                buttonView.UpdateValues(controller.InputDevice);
            }
            foreach (var dPadView in Model.InputDPadViews)
            {
                dPadView.UpdateValues(controller.InputDevice);
            }
        }

        private void CreateMappingControls()
        {
            foreach (var xInputType in XInputHelper.Instance.Buttons)
            {
                Model.MapperButtonViews.Add(new MappingView(new MappingViewModel(new MappingModel(), controller, xInputType)));
            }
            foreach (var xInputType in XInputHelper.Instance.DPad)
            {
                Model.MapperDPadViews.Add(new MappingView(new MappingViewModel(new MappingModel(), controller, xInputType)));
            }
            foreach (var xInputType in XInputHelper.Instance.Axes)
            {
                Model.MapperAxisViews.Add(new MappingView(new MappingViewModel(new MappingModel(), controller, xInputType)));
            }
        }

        private void CreateXInputControls()
        {
            foreach (var buttonInput in controller.XInput.Sources.Where(s => s.Type == InputSourceTypes.Button))
            {
                Model.XInputButtonViews.Add(new ButtonView(new ButtonViewModel(new ButtonModel(), buttonInput)));
            }
            Model.XInputDPadViews.Add(new DPadView(new DPadViewModel(new DPadModel(), 0, false)));
            var lx = controller.XInput.Sources.OfType<XOutputSource>().First(s => s.XInputType == XInputTypes.LX);
            var ly = controller.XInput.Sources.OfType<XOutputSource>().First(s => s.XInputType == XInputTypes.LY);
            var rx = controller.XInput.Sources.OfType<XOutputSource>().First(s => s.XInputType == XInputTypes.RX);
            var ry = controller.XInput.Sources.OfType<XOutputSource>().First(s => s.XInputType == XInputTypes.RY);
            var l2 = controller.XInput.Sources.OfType<XOutputSource>().First(s => s.XInputType == XInputTypes.L2);
            var r2 = controller.XInput.Sources.OfType<XOutputSource>().First(s => s.XInputType == XInputTypes.R2);
            Model.XInputAxisViews.Add(new Axis2DView(new Axis2DViewModel(new Axis2DModel(), lx, ly)));
            Model.XInputAxisViews.Add(new Axis2DView(new Axis2DViewModel(new Axis2DModel(), rx, ry)));
            Model.XInputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), l2)));
            Model.XInputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), r2)));
        }

        private void UpdateXInputControls()
        {
            foreach (var axisView in Model.XInputAxisViews)
            {
                axisView.UpdateValues(controller.XInput);
            }
            foreach (var buttonView in Model.XInputButtonViews)
            {
                buttonView.UpdateValues(controller.XInput);
            }
            foreach (var dPadView in Model.XInputDPadViews)
            {
                dPadView.UpdateValues(controller.XInput);
            }
        }

        private void SetForceFeedback()
        {
            if (controller.ForceFeedbackSupported)
            {
                Model.ForceFeedbackText = "";
                Model.ForceFeedbackEnabled = controller.InputDevice.InputConfiguration.ForceFeedback;
                Model.ForceFeedbackAvailable = true;
            }
            else
            {
                Model.ForceFeedbackEnabled = false;
                Model.ForceFeedbackAvailable = false;
                if (controller.InputDevice.ForceFeedbackCount > 0)
                {
                    Model.ForceFeedbackText = "ForceFeedbackVigemOnly";
                }
                else
                {
                    Model.ForceFeedbackText = "ForceFeedbackUnsupported";
                }
            }
        }

        private void CreateInputAxes()
        {
            var axes = controller.InputDevice.Sources.Where(s => InputSourceTypes.Axis.HasFlag(s.Type)).ToArray();
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
