using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input;
using XOutput.Input.XInput;
using XOutput.UI.Component;

namespace XOutput.UI.View
{
    public class ControllerSettingsViewModel
    {
        private readonly ControllerSettingsModel model = new ControllerSettingsModel();

        public ControllerSettingsModel Model { get { return model; } }
        public bool IsConrtollerConnected { get { return controller.InputDevice.Connected; } }

        private readonly GameController controller;

        public ControllerSettingsViewModel(GameController controller)
        {
            this.controller = controller;
            Model.Title = controller.DisplayName;
            createInputControls();
            createMappingControls();
            createXInputControls();
        }

        private void createInputControls()
        {
            foreach (var buttonInput in controller.InputDevice.GetButtons())
            {
                var inputButtonView = new ButtonView(buttonInput);
                Model.InputButtonViews.Add(inputButtonView);
            }
            if (controller.InputDevice.HasAxes)
            {
                foreach (var axisInput in controller.InputDevice.GetAxes())
                {
                    var inputAxisView = new AxisView(axisInput);
                    Model.InputAxisViews.Add(inputAxisView);
                }
            }
        }
        public void updateInputControls()
        {
            foreach (var axisView in Model.InputAxisViews)
            {
                axisView.Value = (int)(controller.InputDevice.Get(axisView.Type) * 1000);
            }
            foreach (var buttonView in Model.InputButtonViews)
            {
                buttonView.Value = controller.InputDevice.Get(buttonView.Type) > 0.5;
            }
            if (controller.InputDevice.HasDPad)
            {
                Model.DPadText = controller.InputDevice.DPad.ToString();
            }
            else
            {
                Model.DPadText = "This device has no DPad";
            }
        }

        private void createMappingControls()
        {
            foreach (var xInputType in XInputHelper.GetButtons())
            {
                var mappingView = new MappingView(controller.InputDevice, xInputType, controller.Mapper.GetMapping(xInputType));
                Model.MapperButtonViews.Add(mappingView);
            }
            foreach (var xInputType in XInputHelper.GetAxes())
            {
                var mappingView = new MappingView(controller.InputDevice, xInputType, controller.Mapper.GetMapping(xInputType));
                Model.MapperAxisViews.Add(mappingView);
            }
            if (controller.InputDevice.HasDPad)
            {
                Model.MapperDPadText = "DPad is automatically mapped";
            }
            else
            {
                Model.MapperDPadText = "This device has no DPad";
            }
        }

        private void createXInputControls()
        {
            foreach (var buttonInput in XInputHelper.GetButtons())
            {
                var inputButtonView = new ButtonView(buttonInput);
                Model.XInputButtonViews.Add(inputButtonView);
            }
            foreach (var axisInput in XInputHelper.GetAxes())
            {
                var inputAxisView = new AxisView(axisInput);
                Model.XInputAxisViews.Add(inputAxisView);
            }
        }
        public void updateXInputControls()
        {
            foreach (var axisView in Model.XInputAxisViews)
            {
                axisView.Value = (int)(controller.XInput.Get((XInputTypes)axisView.Type) * 1000);
            }
            foreach (var buttonView in Model.XInputButtonViews)
            {
                buttonView.Value = controller.XInput.GetBool((XInputTypes)buttonView.Type);
            }
            Model.XDPadText = controller.XInput.GetDPad().ToString();
        }

        public void AutoConfigure()
        {
            new AutoConfigureWindow(controller).ShowDialog();
            foreach(var view in model.MapperAxisViews.Concat(model.MapperButtonViews))
            {
                view.Refresh();
            }
        }
    }
}
