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

        public void ConfigureAll()
        {
            var types = XInputHelper.Instance.Values;
            if (controller.InputDevice.HasDPad)
            {
                types = types.Where(t => !t.IsDPad());
            }
            new AutoConfigureWindow(controller, types.ToArray()).ShowDialog();
            foreach(var v in Model.MapperAxisViews.Concat(Model.MapperButtonViews).Concat(Model.MapperDPadViews))
            {
                v.Refresh();
            }
        }

        private void createInputControls()
        {
            foreach (var buttonInput in controller.InputDevice.Buttons)
            {
                Model.InputButtonViews.Add(new ButtonView(buttonInput));
            }
            foreach (var axisInput in controller.InputDevice.Axes)
            {
                Model.InputAxisViews.Add(new AxisView(axisInput));
            }
            foreach (var sliderInput in controller.InputDevice.Sliders)
            {
                Model.InputAxisViews.Add(new AxisView(sliderInput));
            }
            if (controller.InputDevice.HasDPad)
            {
                Model.InputDPadViews.Add(new DPadView());
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
                foreach (var dPadView in Model.InputDPadViews)
                {
                    dPadView.updateValues(controller.InputDevice);
                }
            }
        }

        private void createMappingControls()
        {
            foreach (var xInputType in XInputHelper.Instance.Buttons)
            {
                Model.MapperButtonViews.Add(new MappingView(controller, xInputType));
            }
            if (!controller.InputDevice.HasDPad)
            {
                foreach (var xInputType in XInputHelper.Instance.DPad)
                {
                    Model.MapperDPadViews.Add(new MappingView(controller, xInputType));
                }
            }
            foreach (var xInputType in XInputHelper.Instance.Axes)
            {
                Model.MapperAxisViews.Add(new MappingView(controller, xInputType));
            }
        }

        private void createXInputControls()
        {
            foreach (var buttonInput in XInputHelper.Instance.Buttons)
            {
                Model.XInputButtonViews.Add(new ButtonView(buttonInput));
            }
            Model.XInputDPadViews.Add(new DPadView());
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
            foreach (var dPadView in Model.XInputDPadViews)
            {
                dPadView.updateValues(controller.XInput);
            }
        }
    }
}
