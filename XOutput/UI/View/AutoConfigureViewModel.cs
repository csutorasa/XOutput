using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private XInputTypes xInputType;
        private Enum[] inputTypes;

        public AutoConfigureViewModel(GameController controller, XInputTypes valueToRead)
        {
            this.controller = controller;
            xInputType = valueToRead;
            model = new AutoConfigureModel();
            inputTypes = controller.InputDevice.GetButtons().Concat(controller.InputDevice.GetAxes()).ToArray();
        }

        public void Initialize()
        {
            foreach (var type in inputTypes)
            {
                referenceValues[type] = controller.InputDevice.Get(type);
            }
            controller.InputDevice.InputChanged += readValues;
            Model.LabelText = $"Waiting for input for XInput.{xInputType}";
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

        public void SaveValues()
        {
            if (Model.MaxType != null)
            {
                MapperData md = controller.Mapper.GetMapping(xInputType);
                md.InputType = Model.MaxType;
                if (!Model.Invert)
                {
                    md.MinValue = Model.MinValue / 100;
                    md.MaxValue = Model.MaxValue / 100;
                }
                else
                {
                    md.MaxValue = Model.MinValue / 100;
                    md.MinValue = Model.MaxValue / 100;
                }
            }
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
