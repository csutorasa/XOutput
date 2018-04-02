using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input;

namespace XOutput.UI.Component
{
    public class ControllerModel : ModelBase
    {
        private GameController controller;
        public GameController Controller
        {
            get { return controller; }
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
            get { return buttonText; }
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
            get { return started; }
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
            get { return canStart; }
            set
            {
                if (canStart != value)
                {
                    canStart = value;
                    OnPropertyChanged(nameof(CanStart));
                }
            }
        }
        public string DisplayName { get { return Controller.ToString(); } }
    }
}
