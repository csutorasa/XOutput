using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XOutput.Input.XInput;
using XOutput.UI.Component;

namespace XOutput.UI.View
{
    public class AutoConfigureModel : ModelBase
    {
        private XInputTypes xInput;
        public XInputTypes XInput
        {
            get { return xInput; }
            set { if (xInput != value) { xInput = value; OnPropertyChanged(nameof(XInput)); } }
        }
        private bool isAuto = true;
        public bool IsAuto
        {
            get { return isAuto; }
            set { if (isAuto != value) { isAuto = value; OnPropertyChanged(nameof(IsAuto)); if (value) { MaxType = null; } } }
        }
        private bool highlight;
        public bool Highlight
        {
            get { return highlight; }
            set { if (highlight != value) { highlight = value; OnPropertyChanged(nameof(Highlight)); } }
        }
        private Enum maxType;
        public Enum MaxType
        {
            get { return maxType; }
            set { if (maxType != value) { maxType = value; OnPropertyChanged(nameof(MaxType)); } }
        }
        private double minValue;
        public double MinValue
        {
            get { return minValue; }
            set { if (minValue != value) { minValue = value; OnPropertyChanged(nameof(MinValue)); } }
        }
        private double maxValue;
        public double MaxValue
        {
            get { return maxValue; }
            set { if (maxValue != value) { maxValue = value; OnPropertyChanged(nameof(MaxValue)); } }
        }
        private Visibility buttonsVisibility;
        public Visibility ButtonsVisibility
        {
            get { return buttonsVisibility; }
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
            get { return timerMaxValue; }
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
            get { return timerValue; }
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
            get { return timerVisibility; }
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
