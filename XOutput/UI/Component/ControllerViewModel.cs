using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices;
using XOutput.UI.Windows;

namespace XOutput.UI.Component
{
    public class ControllerViewModel : ViewModelBase<ControllerModel>
    {
        private readonly Action<string> log;

        public ControllerViewModel(ControllerModel model, GameController controller, Action<string> log) : base(model)
        {
            this.log = log;
            Model.Controller = controller;
            Model.ButtonText = "Start";
        }

        public void Edit()
        {
            var controllerSettingsWindow = new ControllerSettingsWindow(new ControllerSettingsViewModel(new ControllerSettingsModel(), Model.Controller), Model.Controller);
            controllerSettingsWindow.ShowDialog();
        }

        public void StartStop()
        {
            if (!Model.Started)
            {
                Start();
            }
            else
            {
                Model.Controller.Stop();
            }
        }

        public void Start()
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
        }
    }
}
