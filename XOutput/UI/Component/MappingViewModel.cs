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

namespace XOutput.UI.Component
{
    public class MappingViewModel : ModelBase
    {

        private XInputTypes _xInputType;
        public XInputTypes XInputType
        {
            get { return _xInputType; }
            set
            {
                if (_xInputType != value)
                {
                    _xInputType = value;
                    OnPropertyChanged(nameof(XInputType));
                }
            }
        }

        private ObservableCollection<Enum> inputs = new ObservableCollection<Enum>();
        public ObservableCollection<Enum> Inputs { get { return inputs; } }
        private Enum _selectedInput;
        public Enum SelectedInput
        {
            get { return _selectedInput; }
            set
            {
                if (_selectedInput != value)
                {
                    _selectedInput = value;
                    mapperData.InputType = _selectedInput;
                    OnPropertyChanged(nameof(SelectedInput));
                }
            }
        }

        private decimal? _min;
        public decimal? Min
        {
            get { return _min; }
            set
            {
                if (_min != value)
                {
                    _min = value;
                    if(_min.HasValue)
                        mapperData.MinValue = (double)_min / 100;
                    OnPropertyChanged(nameof(Min));
                }
            }
        }

        private decimal? _max;
        public decimal? Max
        {
            get { return _max; }
            set
            {
                if (_max != value)
                {
                    _max = value;
                    if (_max.HasValue)
                        mapperData.MaxValue = (double)_max / 100;
                    OnPropertyChanged(nameof(Max));
                }
            }
        }

        private MapperData mapperData;

        public MappingViewModel(IInputDevice device, MapperData mapperData)
        {
            this.mapperData = mapperData;
            foreach (var directInput in device.GetButtons())
            {
                Inputs.Add(directInput);
            }
            foreach (var directInput in device.GetAxes())
            {
                Inputs.Add(directInput);
            }
            if (mapperData != null)
            {
                _min = (decimal)mapperData.MinValue * 100;
                _max = (decimal)mapperData.MaxValue * 100;
                if (mapperData.InputType == null)
                    mapperData.InputType = device.GetButtons().FirstOrDefault();
                _selectedInput = mapperData.InputType;
            }
        }
    }
}
