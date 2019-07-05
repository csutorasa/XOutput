using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using XOutput.Devices;
using XOutput.Devices.Input;
using XOutput.Devices.Mapper;
using XOutput.Devices.XInput;
using XOutput.Tools;

namespace XOutput.UI.Windows
{
    public class AutoConfigureViewModel : ViewModelBase<AutoConfigureModel>
    {
        private const int WaitTime = 5000;
        private const int ShortAxisWaitTime = 3000;
        private const int ShortWaitTime = 1000;
        private const int BlinkTime = 500;
        private readonly Dictionary<InputSource, double> referenceValues = new Dictionary<InputSource, double>();
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly IEnumerable<IInputDevice> inputDevices;
        private readonly InputMapper mapper;
        private readonly XInputTypes[] valuesToRead;
        private XInputTypes xInputType;
        private readonly InputSource[] inputTypes;
        private DateTime lastTime;

        public AutoConfigureViewModel(AutoConfigureModel model, IEnumerable<IInputDevice> inputDevices, InputMapper mapper, XInputTypes[] valuesToRead) : base(model)
        {
            this.mapper = mapper;
            this.inputDevices = inputDevices;
            this.valuesToRead = valuesToRead;
            xInputType = valuesToRead.First();
            if (valuesToRead.Length > 1)
            {
                Model.ButtonsVisibility = System.Windows.Visibility.Collapsed;
                Model.TimerVisibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Model.ButtonsVisibility = System.Windows.Visibility.Visible;
                Model.TimerVisibility = System.Windows.Visibility.Collapsed;
            }
            inputTypes = inputDevices.SelectMany(i => i.Sources).ToArray();
            timer.Interval = TimeSpan.FromMilliseconds(BlinkTime);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Model.Highlight = !Model.Highlight;
        }

        public void Initialize()
        {
            ReadReferenceValues();
            foreach (var inputDevice in inputDevices)
            {
                inputDevice.InputChanged += ReadValues;
            }
            Model.XInput = xInputType;
            SetTime(false);
        }

        protected void ReadReferenceValues()
        {
            foreach (var type in inputTypes)
            {
                foreach (var inputDevice in inputDevices)
                {
                    referenceValues[type] = inputDevice.Get(type);
                }
            }
        }

        /// <summary>
        /// Reads the current values, and if the values have changed enough saves them.
        /// </summary>
        private void ReadValues(object sender, DeviceInputChangedEventArgs e)
        {
            var inputDevice = e.Device;
            InputSource maxType = null;
            double maxDiff = 0;
            foreach (var type in e.ChangedValues)
            {
                double oldValue = referenceValues[type];
                double newValue = inputDevice.Get(type);
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
            MapperData md = mapper.GetMapping(xInputType);
            if (md.InputType == null)
            {
                md.Source = inputTypes.First();
            }
            md.MinValue = Model.XInput.GetDisableValue();
            md.MaxValue = Model.XInput.GetDisableValue();
            return Next();
        }

        public bool SaveValues()
        {
            if (Model.MaxType != null)
            {
                MapperData md = mapper.GetMapping(xInputType);
                md.Source = Model.MaxType;
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
            foreach (var inputDevice in inputDevices)
            {
                inputDevice.InputChanged -= ReadValues;
            }
            timer.Stop();
        }

        protected void SetTime(bool shortTime)
        {
            Model.TimerValue = 0;
            if (shortTime)
            {
                Model.TimerMaxValue = xInputType.IsAxis() ? ShortAxisWaitTime : ShortWaitTime;
            }
            else
            {
                Model.TimerMaxValue = WaitTime;
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
            double current = Model.MaxType.InputDevice.Get(Model.MaxType);

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
            double current = Model.MaxType.InputDevice.Get(Model.MaxType);
            double reference = referenceValues[Model.MaxType];

            double min = Math.Min(current, reference);
            Model.MinValue = Math.Round(min * 100);

            double max = Math.Max(current, reference);
            Model.MaxValue = Math.Round(max * 100);

            SetTime(true);
        }
    }
}
