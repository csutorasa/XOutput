using XOutput.Devices;
using XOutput.Devices.Input;
using XOutput.Devices.Mapper;
using XOutput.Devices.XInput;
using XOutput.Tools;
using XOutput.UI.Windows;

namespace XOutput.UI.Component
{
    public class MappingViewModel : ViewModelBase<MappingModel>
    {
        private readonly GameController controller;

        public MappingViewModel(MappingModel model, GameController controller, XInputTypes inputType) : base(model)
        {
            this.controller = controller;
            Model.XInputType = inputType;
            var mapperData = GetMapperData();
            Model.MapperData = mapperData;
            SetSelected(mapperData);
        }

        public void Configure()
        {
            new AutoConfigureWindow(new AutoConfigureViewModel(new AutoConfigureModel(), InputDevices.Instance.GetDevices(), controller.Mapper, new XInputTypes[] { Model.XInputType }), false).ShowDialog();
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
            return controller.Mapper.GetMapping(Model.XInputType).Mappers[0]; // TODO
        }

        protected void SetSelected(MapperData mapperData)
        {
            if (Helper.DoubleEquals(mapperData.MinValue, Model.XInputType.GetDisableValue()) && Helper.DoubleEquals(mapperData.MaxValue, Model.XInputType.GetDisableValue()))
            {
                Model.SelectedInput = DisabledInputSource.Instance;
                Model.ConfigVisibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                Model.SelectedInput = mapperData.Source;
                Model.ConfigVisibility = System.Windows.Visibility.Visible;
            }
            if (mapperData.Source == null)
            {
                Model.ConfigVisibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                SelectionChanged(Model.SelectedInput);
            }
        }

        protected void SelectionChanged(InputSource type)
        {
            if (type.Type == InputSourceTypes.Disabled)
            {
                Model.Min = (decimal)(100 * Model.XInputType.GetDisableValue());
                Model.Max = (decimal)(100 * Model.XInputType.GetDisableValue());
                Model.ConfigVisibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                Model.MapperData.Source = type;
                Model.ConfigVisibility = System.Windows.Visibility.Visible;
            }
            Controllers.Instance.Update(controller, InputDevices.Instance.GetDevices());
        }
    }
}
