using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XOutput.Devices;
using XOutput.Devices.XInput;

namespace XOutput.UI.Windows
{
    public class AutoConfigureModel : ModelBase
    {
        private XInputTypes xInput;
        public XInputTypes XInput
        {
            get => xInput;
            set { if (xInput != value) { xInput = value; OnPropertyChanged(nameof(XInput)); } }
        }
        private bool isAuto = true;
        public bool IsAuto
        {
            get => isAuto;
            set { if (isAuto != value) { isAuto = value; OnPropertyChanged(nameof(IsAuto)); if (value) { MaxType = null; } } }
        }
        private bool highlight;
        public bool Highlight
        {
            get => highlight;
            set { if (highlight != value) { highlight = value; OnPropertyChanged(nameof(Highlight)); } }
        }
        private InputSource maxType;
        public InputSource MaxType
        {
            get => maxType;
            set { if (maxType != value) { maxType = value; OnPropertyChanged(nameof(MaxType)); } }
        }
        private double minValue;
        public double MinValue
        {
            get => minValue;
            set { if (minValue != value) { minValue = value; OnPropertyChanged(nameof(MinValue)); } }
        }
        private double maxValue;
        public double MaxValue
        {
            get => maxValue;
            set { if (maxValue != value) { maxValue = value; OnPropertyChanged(nameof(MaxValue)); } }
        }
        private Visibility buttonsVisibility;
        public Visibility ButtonsVisibility
        {
            get => buttonsVisibility;
            set
            {
                if (buttonsVisibility != value)
                {
                    buttonsVisibility = value;
                    OnPropertyChanged(nameof(ButtonsVisibility));
                }
            }
        }
        private double timerMaxValue;
        public double TimerMaxValue
        {
            get => timerMaxValue;
            set
            {
                if (timerMaxValue != value)
                {
                    timerMaxValue = value;
                    OnPropertyChanged(nameof(TimerMaxValue));
                }
            }
        }
        private double timerValue;
        public double TimerValue
        {
            get => timerValue;
            set
            {
                if (timerValue != value)
                {
                    timerValue = value;
                    OnPropertyChanged(nameof(TimerValue));
                }
            }
        }
        private Visibility timerVisibility;
        public Visibility TimerVisibility
        {
            get => timerVisibility;
            set
            {
                if (timerVisibility != value)
                {
                    timerVisibility = value;
                    OnPropertyChanged(nameof(TimerVisibility));
                }
            }
        }
    }
}
