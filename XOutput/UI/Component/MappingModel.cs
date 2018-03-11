using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XOutput.Input;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.Input.XInput;
using XOutput.UI.View;

namespace XOutput.UI
{
    public class MappingModel : ModelBase
    {
        public event Action<Enum> SelectedInputChanged;

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

        private readonly ObservableCollection<Enum> inputs = new ObservableCollection<Enum>();
        public ObservableCollection<Enum> Inputs { get { return inputs; } }

        private Enum selectedInput;
        public Enum SelectedInput
        {
            get { return selectedInput; }
            set
            {
                if (selectedInput != value)
                {
                    selectedInput = value;
                    OnPropertyChanged(nameof(SelectedInput));
                    SelectedInputChanged?.Invoke(value);
                }
            }
        }

        public decimal? Min
        {
            get { return (decimal)mapperData.MinValue * 100; }
            set
            {
                if ((decimal)mapperData.MinValue != value)
                {
                    mapperData.MinValue = (double)(value ?? 0) / 100;
                    OnPropertyChanged(nameof(Min));
                }
            }
        }

        public decimal? Max
        {
            get { return (decimal)mapperData.MaxValue * 100; }
            set
            {
                if ((decimal)mapperData.MaxValue != value)
                {
                    mapperData.MaxValue = (double)(value ?? 100) / 100;
                    OnPropertyChanged(nameof(Max));
                }
            }
        }

        private Visibility configVisibility;
        public Visibility ConfigVisibility
        {
            get { return configVisibility; }
            set
            {
                if (configVisibility != value)
                {
                    configVisibility = value;
                    OnPropertyChanged(nameof(ConfigVisibility));
                }
            }
        }

        public MapperData MapperData => mapperData;

        private readonly MapperData mapperData;

        public MappingModel(MapperData mapperData)
        {
            this.mapperData = mapperData;
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(XInputType));
            OnPropertyChanged(nameof(SelectedInput));
            OnPropertyChanged(nameof(Min));
            OnPropertyChanged(nameof(Max));
        }
    }
}
