using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices;
using XOutput.Devices.XInput;
using XOutput.Devices.XInput.Settings;
using XOutput.Tools;
using XOutput.UI.Windows;

namespace XOutput.UI.Component
{
    enum MappingTypes
    {
        Disabled
    }

    public class MappingViewModel : ViewModelBase<MappingModel>
    {
        private GameController controller;

        public MappingViewModel(MappingModel model, GameController controller, XInputTypes inputType) : base(model)
        {
            this.controller = controller;
            var mapperData = controller.XInput.GetMapping(inputType);
            Model.XInputType = inputType;
            //var device = controller.InputDevice;
            Model.Inputs.Add(MappingTypes.Disabled);
            /*foreach (var directInput in device.Buttons)
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
                mapperData.InputType = device.Buttons.FirstOrDefault();*/
            Model.MapperData = mapperData;
            SetSelected(mapperData);
        }

        public void Configure()
        {
            new AutoConfigureWindow(new AutoConfigureViewModel(new AutoConfigureModel(), controller, new XInputTypes[] { Model.XInputType }), false).ShowDialog();
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

        protected MapperSettings GetMapperData()
        {
            return controller.GetMapping(Model.XInputType);
        }

        protected void SetSelected(MapperSettings mapperData)
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
            SelectionChanged(Model.SelectedInput);
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
