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
    public class ControllerSettingsViewModel : ViewModelBase<ControllerSettingsModel>
    {
        public bool IsConrtollerConnected { get { return controller.InputDevice.Connected; } }

        private readonly GameController controller;

        public ControllerSettingsViewModel(GameController controller)
        {
            this.controller = controller;
            model = new ControllerSettingsModel();
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
            foreach (var xInputType in XInputHelper.Buttons)
            {
                var mappingView = new MappingView(controller, xInputType);
                Model.MapperButtonViews.Add(mappingView);
            }
            foreach (var xInputType in XInputHelper.Axes)
            {
                var mappingView = new MappingView(controller, xInputType);
                Model.MapperAxisViews.Add(mappingView);
            }
            if (controller.InputDevice.HasDPad)
            {
                Model.MapperDPadText = "AutomaticDPad";
            }
            else
            {
                Model.MapperDPadText = "NoDPad";
            }
        }

        private void createXInputControls()
        {
            foreach (var buttonInput in XInputHelper.Buttons)
            {
                var inputButtonView = new ButtonView(buttonInput);
                Model.XInputButtonViews.Add(inputButtonView);
            }
            foreach (var axisInput in XInputHelper.Axes)
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
    }
}
