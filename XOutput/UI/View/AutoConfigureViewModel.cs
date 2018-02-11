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
        private readonly Dictionary<Enum, double> referenceValues = new Dictionary<Enum, double>();
        private readonly GameController controller;
        private readonly XInputTypes[] valuesToRead;
        private XInputTypes xInputType;
        private Enum[] inputTypes;

        public AutoConfigureViewModel(GameController controller, XInputTypes[] valuesToRead)
        {
            this.controller = controller;
            this.valuesToRead = valuesToRead;
            xInputType = valuesToRead.First();
            model = new AutoConfigureModel();
            Model.ButtonsVisibility = valuesToRead.Length > 1 ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            inputTypes = controller.InputDevice.GetButtons().Concat(controller.InputDevice.GetAxes()).ToArray();
            Model.CenterVisibility = xInputType.IsAxis() ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public void Initialize()
        {
            readReferenceValues();
            controller.InputDevice.InputChanged += readValues;
            Model.LabelText = $"Waiting for input for XInput.{xInputType}";
        }

        protected void readReferenceValues()
        {
            foreach (var type in inputTypes)
            {
                referenceValues[type] = controller.InputDevice.Get(type);
            }
        }

        /// <summary>
        /// Reads the current values, and if the values have changed enough saves them.
        /// </summary>
        private void readValues()
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
                    calculateStartValues();
                }
            }
            if (Model.MaxType != null)
            {
                calculateValues();
            }
        }
        
        public bool SaveCenterValues()
        {
            MapperData md = controller.Mapper.GetMapping(xInputType);
            if (md.InputType == null)
            {
                md.InputType = inputTypes.First();
            }
            md.MinValue = 0.5;
            md.MaxValue = 0.5;
            return next();
        }

        public bool SaveDisableValues()
        {
            MapperData md = controller.Mapper.GetMapping(xInputType);
            if(md.InputType == null)
            {
                md.InputType = inputTypes.First();
            }
            md.MinValue = 0;
            md.MaxValue = 0;
            return next();
        }

        public bool SaveValues()
        {
            if (Model.MaxType != null)
            {
                MapperData md = controller.Mapper.GetMapping(xInputType);
                md.InputType = Model.MaxType;
                md.MinValue = Model.MinValue / 100;
                md.MaxValue = Model.MaxValue / 100;
                return next();
            }
            else
            {
                return SaveDisableValues();
            }
        }

        public void Close()
        {
            controller.InputDevice.InputChanged -= readValues;
        }

        protected bool next()
        {
            Model.MaxType = null;
            int index = Array.IndexOf(valuesToRead, xInputType);
            if(index + 1 < valuesToRead.Length)
            {
                xInputType = valuesToRead[index + 1];
                Model.LabelText = $"Waiting for input for XInput.{xInputType}";
                return true;
            }
            return false;
        }

        private void calculateValues()
        {
            double current = controller.InputDevice.Get(Model.MaxType);

            double min = Math.Min(current, Model.MinValue / 100);
            Model.MinValue = Math.Round(min * 100);

            double max = Math.Max(current, Model.MaxValue / 100);
            Model.MaxValue = Math.Round(max * 100);
        }

        private void calculateStartValues()
        {
            double current = controller.InputDevice.Get(Model.MaxType);
            double reference = referenceValues[Model.MaxType];

            double min = Math.Min(current, reference);
            Model.MinValue = Math.Round(min * 100);

            double max = Math.Max(current, reference);
            Model.MaxValue = Math.Round(max * 100);
        }
    }
}
