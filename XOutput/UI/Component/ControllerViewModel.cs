using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.Input.XInput;
using XOutput.UI.View;

namespace XOutput.UI.Component
{
    public class ControllerViewModel : ViewModelBase<ControllerModel>
    {
        private readonly Action<string> log;

        public ControllerViewModel(GameController controller, Action<string> log)
        {
            this.log = log;
            model = new ControllerModel();
            Model.Controller = controller;
            Model.ButtonText = "Start";
        }

        public void Edit()
        {
            var controllerSettingsWindow = new ControllerSettings(Model.Controller);
            controllerSettingsWindow.ShowDialog();
        }

        public void StartStop()
        {
            if (!Model.Started)
            {
                int controllerCount = 0;
                controllerCount = Model.Controller.Start(() =>
                {
                    Model.ButtonText = "Start";
                    log?.Invoke(string.Format(LanguageModel.Instance.Translate("EmulationStopped"), Model.Controller.DisplayName));
                    Model.Started = false;
                });
                if (controllerCount != 0)
                {
                    Model.ButtonText = "Stop";
                    log?.Invoke(string.Format(LanguageModel.Instance.Translate("EmulationStarted"), Model.Controller.DisplayName, controllerCount));
                }
                Model.Started = controllerCount != 0;
            }
            else
            {
                Model.Controller.Stop();
            }
        }
    }
}
