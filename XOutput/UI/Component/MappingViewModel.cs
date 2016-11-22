using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.Input.XInput;

namespace XOutput.UI.Component
{
    public class MappingViewModel : ViewModelBase
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

        private ObservableCollection<DirectInputTypes> _directInputs = new ObservableCollection<DirectInputTypes>();
        public ObservableCollection<DirectInputTypes> DirectInputs { get { return _directInputs; } }
        private DirectInputTypes _selectedDirectInput;
        public DirectInputTypes SelectedDirectInput
        {
            get { return _selectedDirectInput; }
            set
            {
                if (_selectedDirectInput != value)
                {
                    _selectedDirectInput = value;
                    mapperData.InputType = _selectedDirectInput;
                    OnPropertyChanged(nameof(SelectedDirectInput));
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

        private MapperData<DirectInputTypes> mapperData;

        public MappingViewModel(MapperData<DirectInputTypes> mapperData)
        {
            this.mapperData = mapperData;
            foreach (var directInput in DirectInputHelper.GetAll())
            {
                DirectInputs.Add(directInput);
            }
            if (mapperData != null)
            {
                _min = (decimal)mapperData.MinValue * 100;
                _max = (decimal)mapperData.MaxValue * 100;
                if (mapperData.InputType.HasValue)
                    _selectedDirectInput = mapperData.InputType.Value;
            }
        }
    }
}
