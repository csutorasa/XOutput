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
                Model.InputButtonViews.Add(new ButtonView(buttonInput));
            }
            if (controller.InputDevice.HasAxes)
            {
                foreach (var axisInput in controller.InputDevice.GetAxes())
                {
                    Model.InputAxisViews.Add(new AxisView(axisInput));
                }
            }
        }
        public void updateInputControls()
        {
            foreach (var axisView in Model.InputAxisViews)
            {
                axisView.updateValues(controller.InputDevice);
            }
            foreach (var buttonView in Model.InputButtonViews)
            {
                buttonView.updateValues(controller.InputDevice);
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
                Model.MapperButtonViews.Add(new MappingView(controller, xInputType));
            }
            foreach (var xInputType in XInputHelper.Axes)
            {
                Model.MapperAxisViews.Add(new MappingView(controller, xInputType));
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
                Model.XInputButtonViews.Add(new ButtonView(buttonInput));
            }
            Model.XInputAxisViews.Add(new Axis2DView(XInputTypes.LX, XInputTypes.LY));
            Model.XInputAxisViews.Add(new Axis2DView(XInputTypes.RX, XInputTypes.RY));
            Model.XInputAxisViews.Add(new AxisView(XInputTypes.L2));
            Model.XInputAxisViews.Add(new AxisView(XInputTypes.R2));
        }
        public void updateXInputControls()
        {
            foreach (var axisView in Model.XInputAxisViews)
            {
                axisView.updateValues(controller.XInput);
            }
            foreach (var buttonView in Model.XInputButtonViews)
            {
                buttonView.updateValues(controller.XInput);
            }
            Model.XDPadText = controller.XInput.GetDPad().ToString();
        }
    }
}
