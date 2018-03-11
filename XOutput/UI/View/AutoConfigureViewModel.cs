using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using XOutput.Input;
using XOutput.Input.Mapper;
using XOutput.Input.XInput;
using XOutput.UI.Component;

namespace XOutput.UI.View
{
    public class AutoConfigureViewModel : ViewModelBase<AutoConfigureModel>
    {
        private const int WAIT_TIME = 5000;
        private const int SHORT_AXIS_WAIT_TIME = 3000;
        private const int SHORT_WAIT_TIME = 1000;
        private readonly Dictionary<Enum, double> referenceValues = new Dictionary<Enum, double>();
        private readonly GameController controller;
        private readonly XInputTypes[] valuesToRead;
        private XInputTypes xInputType;
        private readonly Enum[] inputTypes;
        private DateTime lastTime;

        public AutoConfigureViewModel(GameController controller, XInputTypes[] valuesToRead)
        {
            this.controller = controller;
            this.valuesToRead = valuesToRead;
            xInputType = valuesToRead.First();
            model = new AutoConfigureModel();
            Model.ButtonsVisibility = valuesToRead.Length > 1 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            Model.TimerVisibility = valuesToRead.Length <= 1 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            inputTypes = controller.InputDevice.Buttons.Concat(controller.InputDevice.Axes).Concat(controller.InputDevice.Sliders).ToArray();
        }

        public void Initialize()
        {
            ReadReferenceValues();
            controller.InputDevice.InputChanged += ReadValues;
            Model.XInput = xInputType;
            SetTime(false);
        }

        protected void ReadReferenceValues()
        {
            foreach (var type in inputTypes)
            {
                referenceValues[type] = controller.InputDevice.Get(type);
            }
        }

        /// <summary>
        /// Reads the current values, and if the values have changed enough saves them.
        /// </summary>
        private void ReadValues()
        {
            Enum maxType = null;
            double maxDiff = 0;
            foreach (var type in inputTypes)
            {
                double oldValue = referenceValues[type];
                double newValue = controller.InputDevice.Get(type);
                double diff = Math.Abs(newValue - oldValue);
                if (diff > maxDiff)
                {
                    maxType = type;
                    maxDiff = diff;
                }
            }
            if (maxDiff > 0.3)
            {
                if (maxType != Model.MaxType)
                {
                    Model.MaxType = maxType;
                    CalculateStartValues();
                }
            }
            if (Model.MaxType != null)
            {
                CalculateValues();
            }
        }

        public bool SaveDisableValues()
        {
            MapperData md = controller.Mapper.GetMapping(xInputType);
            if (md.InputType == null)
            {
                md.InputType = inputTypes.First();
            }
            md.MinValue = Model.XInput.GetDisableValue();
            md.MaxValue = Model.XInput.GetDisableValue();
            return Next();
        }

        public bool SaveValues()
        {
            if (Model.MaxType != null)
            {
                MapperData md = controller.Mapper.GetMapping(xInputType);
                md.InputType = Model.MaxType;
                md.MinValue = Model.MinValue / 100;
                md.MaxValue = Model.MaxValue / 100;
                return Next();
            }
            else
            {
                return SaveDisableValues();
            }
        }

        public bool IncreaseTime()
        {
            Model.TimerValue += DateTime.Now.Subtract(lastTime).TotalMilliseconds;
            lastTime = DateTime.Now;
            return Model.TimerValue > Model.TimerMaxValue;
        }

        public void Close()
        {
            controller.InputDevice.InputChanged -= ReadValues;
        }

        protected void SetTime(bool shortTime)
        {
            Model.TimerValue = 0;
            if (shortTime)
            {
                Model.TimerMaxValue = xInputType.IsAxis() ? SHORT_AXIS_WAIT_TIME : SHORT_WAIT_TIME;
            }
            else
            {
                Model.TimerMaxValue = WAIT_TIME;
            }
            lastTime = DateTime.Now;
        }

        protected bool Next()
        {
            Model.MaxType = null;
            int index = Array.IndexOf(valuesToRead, xInputType);
            SetTime(false);
            if (index + 1 < valuesToRead.Length)
            {
                xInputType = valuesToRead[index + 1];
                Model.XInput = xInputType;
                return true;
            }
            return false;
        }

        private void CalculateValues()
        {
            double current = controller.InputDevice.Get(Model.MaxType);

            double min = Math.Min(current, Model.MinValue / 100);
            double minValue = Math.Round(min * 100);

            double max = Math.Max(current, Model.MaxValue / 100);
            double maxValue = Math.Round(max * 100);

            if (!Helper.DoubleEquals(minValue, Model.MinValue) || !Helper.DoubleEquals(maxValue, Model.MaxValue))
            {
                Model.MinValue = minValue;
                Model.MaxValue = maxValue;
                SetTime(true);
            }
        }

        private void CalculateStartValues()
        {
            double current = controller.InputDevice.Get(Model.MaxType);
            double reference = referenceValues[Model.MaxType];

            double min = Math.Min(current, reference);
            Model.MinValue = Math.Round(min * 100);

            double max = Math.Max(current, reference);
            Model.MaxValue = Math.Round(max * 100);

            SetTime(true);
        }
    }
}
