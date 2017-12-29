using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.Input.XInput;
using XOutput.UI.View;

namespace XOutput.UI.Component
{
    public class MappingViewModel : ViewModelBase<MappingModel>
    {
        private GameController controller;

        public MappingViewModel(GameController controller, XInputTypes inputType)
        {
            this.controller = controller;
            MapperData mapperData = controller.Mapper.GetMapping(inputType);
            model = new MappingModel(mapperData);
            Model.XInputType = inputType;
            IInputDevice device = controller.InputDevice;
            foreach (var directInput in device.GetButtons())
            {
                Model.Inputs.Add(directInput);
            }
            foreach (var directInput in device.GetAxes())
            {
                Model.Inputs.Add(directInput);
            }
            if (mapperData != null && mapperData.InputType == null)
                mapperData.InputType = device.GetButtons().FirstOrDefault();
        }

        public void Configure()
        {
            new AutoConfigureWindow(controller, Model.XInputType).ShowDialog();
        }
    }
}
