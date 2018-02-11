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
        Disabled,
        Centered
    }

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
            if (inputType.IsAxis())
            {
                Model.Inputs.Add(MappingTypes.Centered);
            }
            Model.Inputs.Add(MappingTypes.Disabled);
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
            if (Helper.DoubleEquals(mapperData.MinValue, 0) && Helper.DoubleEquals(mapperData.MaxValue, 0))
            {
                Model.SelectedInput = MappingTypes.Disabled;
                Model.ConfigVisibility = System.Windows.Visibility.Collapsed;
            }
            else if (Helper.DoubleEquals(mapperData.MinValue, 0.5) && Helper.DoubleEquals(mapperData.MaxValue, 0.5))
            {
                Model.SelectedInput = MappingTypes.Centered;
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
                Model.Min = 0;
                Model.Max = 0;
                Model.ConfigVisibility = System.Windows.Visibility.Collapsed;
            }
            else if (type.Equals(MappingTypes.Centered))
            {
                Model.Min = 50;
                Model.Max = 50;
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
