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
        private GameController _controller;
        public GameController Controller
        {
            get { return _controller; }
            set
            {
                if (_controller != value)
                {
                    _controller = value;
                    OnPropertyChanged(nameof(Controller));
                }
            }
        }

        private string _buttonText;
        public string ButtonText
        {
            get { return _buttonText; }
            set
            {
                if (_buttonText != value)
                {
                    _buttonText = value;
                    OnPropertyChanged(nameof(ButtonText));
                }
            }
        }
        private bool _started;
        public bool Started
        {
            get { return _started; }
            set
            {
                if (_started != value)
                {
                    _started = value;
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
