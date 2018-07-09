using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.Input.Settings;

namespace XOutput.UI.Component
{
    public class InputAxisModel : ModelBase
    {
        private readonly InputSettings settings;

        private Enum type;
        public Enum Type
        {
            get => type;
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }
        private int value;
        public int Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }
        private int rawValue;
        public int RawValue
        {
            get => rawValue;
            set
            {
                if (this.rawValue != value)
                {
                    this.rawValue = value;
                    OnPropertyChanged(nameof(RawValue));
                }
            }
        }
        private int max;
        public int Max
        {
            get => max;
            set
            {
                if (max != value)
                {
                    max = value;
                    OnPropertyChanged(nameof(Max));
                }
            }
        }

        public decimal? Deadzone
        {
            get => (decimal)settings.Deadzone * 100;
            set
            {
                if ((decimal)settings.Deadzone != value)
                {
                    settings.Deadzone = (double)(value ?? 100) / 100;
                    OnPropertyChanged(nameof(Deadzone));
                }
            }
        }

        public decimal? AntiDeadzone
        {
            get => (decimal)settings.AntiDeadzone * 100;
            set
            {
                if ((decimal)settings.AntiDeadzone != value)
                {
                    settings.AntiDeadzone = (double)(value ?? 100) / 100;
                    OnPropertyChanged(nameof(AntiDeadzone));
                }
            }
        }

        public InputAxisModel(InputSettings settings)
        {
            this.settings = settings;
        }
    }
}
