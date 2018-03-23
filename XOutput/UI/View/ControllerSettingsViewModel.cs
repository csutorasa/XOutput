using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input;
using XOutput.Input.DirectInput;
using XOutput.Input.XInput;
using XOutput.UI.Component;

namespace XOutput.UI.View
{
    public class ControllerSettingsViewModel : ViewModelBase<ControllerSettingsModel>
    {
        private readonly GameController controller;

        public ControllerSettingsViewModel(ControllerSettingsModel model, GameController controller) : base(model)
        {
            this.controller = controller;
            Model.Title = controller.DisplayName;
            CreateInputControls();
            CreateMappingControls();
            CreateXInputControls();
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
            foreach (var buttonInput in controller.InputDevice.Buttons)
            {
                Model.InputButtonViews.Add(new ButtonView(new ButtonViewModel(new ButtonModel(), buttonInput)));
            }
            var axes = controller.InputDevice.Axes.OfType<DirectInputTypes>();
            if (axes.Contains(DirectInputTypes.Axis1) && axes.Contains(DirectInputTypes.Axis2))
            {
                Model.InputAxisViews.Add(new Axis2DView(new Axis2DViewModel(new Axis2DModel(), DirectInputTypes.Axis1, DirectInputTypes.Axis2)));
            }
            else
            {
                if (axes.Contains(DirectInputTypes.Axis1))
                {
                    Model.InputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), DirectInputTypes.Axis1)));
                }
                if (axes.Contains(DirectInputTypes.Axis2))
                {
                    Model.InputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), DirectInputTypes.Axis2)));
                }
            }
            if (axes.Contains(DirectInputTypes.Axis4) && axes.Contains(DirectInputTypes.Axis5))
            {
                Model.InputAxisViews.Add(new Axis2DView(new Axis2DViewModel(new Axis2DModel(), DirectInputTypes.Axis4, DirectInputTypes.Axis5)));
            }
            else
            {
                if (axes.Contains(DirectInputTypes.Axis4))
                {
                    Model.InputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), DirectInputTypes.Axis4)));
                }
                if (axes.Contains(DirectInputTypes.Axis5))
                {
                    Model.InputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), DirectInputTypes.Axis5)));
                }
            }
            if (axes.Contains(DirectInputTypes.Axis3))
            {
                Model.InputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), DirectInputTypes.Axis3)));
            }
            if (axes.Contains(DirectInputTypes.Axis6))
            {
                Model.InputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), DirectInputTypes.Axis6)));
            }
            foreach (var sliderInput in controller.InputDevice.Sliders)
            {
                Model.InputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), sliderInput)));
            }
            foreach (var dPadInput in Enumerable.Range(0, controller.InputDevice.DPads.Count()))
            {
                Model.InputDPadViews.Add(new DPadView(new DPadViewModel(new DPadModel(), dPadInput)));
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
            if (!controller.InputDevice.DPads.Any())
            {
                foreach (var xInputType in XInputHelper.Instance.DPad)
                {
                    Model.MapperDPadViews.Add(new MappingView(new MappingViewModel(new MappingModel(), controller, xInputType)));
                }
            }
            foreach (var xInputType in XInputHelper.Instance.Axes)
            {
                Model.MapperAxisViews.Add(new MappingView(new MappingViewModel(new MappingModel(), controller, xInputType)));
            }
        }

        private void CreateXInputControls()
        {
            foreach (var buttonInput in XInputHelper.Instance.Buttons)
            {
                Model.XInputButtonViews.Add(new ButtonView(new ButtonViewModel(new ButtonModel(), buttonInput)));
            }
            Model.XInputDPadViews.Add(new DPadView(new DPadViewModel(new DPadModel(), 0)));
            Model.XInputAxisViews.Add(new Axis2DView(new Axis2DViewModel(new Axis2DModel(), XInputTypes.LX, XInputTypes.LY)));
            Model.XInputAxisViews.Add(new Axis2DView(new Axis2DViewModel(new Axis2DModel(), XInputTypes.RX, XInputTypes.RY)));
            Model.XInputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), XInputTypes.L2)));
            Model.XInputAxisViews.Add(new AxisView(new AxisViewModel(new AxisModel(), XInputTypes.R2)));
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
    }
}
