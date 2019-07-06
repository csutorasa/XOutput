using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XOutput.Devices;
using XOutput.Devices.Mapper;
using XOutput.Devices.XInput;

namespace XOutput.UI
{
    public class MappingModel : ModelBase
    {
        private XInputTypes xInputType;
        public XInputTypes XInputType
        {
            get => xInputType;
            set
            {
                if (xInputType != value)
                {
                    xInputType = value;
                    OnPropertyChanged(nameof(XInputType));
                }
            }
        }

        private InputSource selectedInput;
        public InputSource SelectedInput
        {
            get => selectedInput;
            set
            {
                if (selectedInput != value)
                {
                    selectedInput = value;
                    OnPropertyChanged(nameof(SelectedInput));
                }
            }
        }

        public decimal? Min
        {
            get => (decimal)mapperData.MinValue * 100;
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
            get => (decimal)mapperData.MaxValue * 100;
            set
            {
                if ((decimal)mapperData.MaxValue != value)
                {
                    mapperData.MaxValue = (double)(value ?? 100) / 100;
                    OnPropertyChanged(nameof(Max));
                }
            }
        }

        public decimal? Deadzone
        {
            get => (decimal)mapperData.Deadzone * 100;
            set
            {
                if ((decimal)mapperData.Deadzone != value)
                {
                    mapperData.Deadzone = (double)(value ?? 100) / 100;
                    OnPropertyChanged(nameof(Deadzone));
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

        private MapperData mapperData;
        public MapperData MapperData
        {
            get => mapperData;
            set
            {
                if (mapperData != value)
                {
                    mapperData = value;
                    OnPropertyChanged(nameof(MapperData));
                }
            }
        }

        public void Refresh()
        {
            //OnPropertyChanged(nameof(XInputType));
            OnPropertyChanged(nameof(SelectedInput));
            OnPropertyChanged(nameof(Min));
            OnPropertyChanged(nameof(Max));
        }
    }
}
