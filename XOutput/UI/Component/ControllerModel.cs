using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using XOutput.Devices;

namespace XOutput.UI.Component
{
    public class ControllerModel : ModelBase
    {
        private GameController controller;
        public GameController Controller
        {
            get => controller;
            set
            {
                if (controller != value)
                {
                    controller = value;
                    OnPropertyChanged(nameof(Controller));
                }
            }
        }

        private string buttonText;
        public string ButtonText
        {
            get => buttonText;
            set
            {
                if (buttonText != value)
                {
                    buttonText = value;
                    OnPropertyChanged(nameof(ButtonText));
                }
            }
        }
        private bool started;
        public bool Started
        {
            get => started;
            set
            {
                if (started != value)
                {
                    started = value;
                    OnPropertyChanged(nameof(Started));
                }
            }
        }

        private bool canStart;
        public bool CanStart
        {
            get => canStart;
            set
            {
                if (canStart != value)
                {
                    canStart = value;
                    OnPropertyChanged(nameof(CanStart));
                }
            }
        }

        private Brush background;
        public Brush Background
        {
            get => background;
            set
            {
                if (background != value)
                {
                    background = value;
                    OnPropertyChanged(nameof(Background));
                }
            }
        }
        public string DisplayName { get { return Controller.ToString(); } }

        private int selectedOutputIndex;
        public int SelectedOutputIndex
        {
            // get => OutputDevices.Instance.GetDevices().IndexOf(controller.XOutputInterface);
            get => selectedOutputIndex;
            set
            {
                if (selectedOutputIndex != value)
                {
                    selectedOutputIndex = value;
                    OnPropertyChanged(nameof(SelectedOutputIndex));
                }
            }
        }


        public void RefreshName()
        {
            OnPropertyChanged(nameof(DisplayName));
        }
    }
}
