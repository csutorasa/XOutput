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
    enum MappingTypes
    {
        Disabled
    }

    public class MappingViewModel : ViewModelBase<MappingModel>
    {
        private GameController controller;

        public MappingViewModel(GameController controller, XInputTypes inputType)
        {
            this.controller = controller;
            var mapperData = controller.Mapper.GetMapping(inputType);
            model = new MappingModel(mapperData);
            Model.XInputType = inputType;
            var device = controller.InputDevice;
            Model.Inputs.Add(MappingTypes.Disabled);
            foreach (var directInput in device.Buttons)
            {
                Model.Inputs.Add(directInput);
            }
            foreach (var directInput in device.Axes)
            {
                Model.Inputs.Add(directInput);
            }
            foreach (var directInput in device.Sliders)
            {
                Model.Inputs.Add(directInput);
            }
            if (mapperData != null && mapperData.InputType == null)
                mapperData.InputType = device.Buttons.FirstOrDefault();
            SetSelected(mapperData);
            Model.SelectedInputChanged += SelectionChanged;
        }

        public void Configure()
        {
            new AutoConfigureWindow(controller, Model.XInputType).ShowDialog();
            SetSelected(GetMapperData());
        }

        public void Invert()
        {
            decimal? temp = Model.Max;
            Model.Max = Model.Min;
            Model.Min = temp;
        }

        public void Refresh()
        {
            Model.Refresh();
            SetSelected(GetMapperData());
        }

        protected MapperData GetMapperData()
        {
            return controller.Mapper.GetMapping(Model.XInputType);
        }

        protected void SetSelected(MapperData mapperData)
        {
            if (Helper.DoubleEquals(mapperData.MinValue, Model.XInputType.GetDisableValue()) && Helper.DoubleEquals(mapperData.MaxValue, Model.XInputType.GetDisableValue()))
            {
                Model.SelectedInput = MappingTypes.Disabled;
                Model.ConfigVisibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                Model.SelectedInput = mapperData.InputType;
                Model.ConfigVisibility = System.Windows.Visibility.Visible;
            }
        }

        protected void SelectionChanged(Enum type)
        {
            if (type.Equals(MappingTypes.Disabled))
            {
                Model.Min = (decimal)(100 * Model.XInputType.GetDisableValue());
                Model.Max = (decimal)(100 * Model.XInputType.GetDisableValue());
                Model.ConfigVisibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                Model.MapperData.InputType = type;
                Model.ConfigVisibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
