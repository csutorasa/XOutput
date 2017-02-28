using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XOutput.Input;
using XOutput.UI.Component;
using XOutput.UI.Resources;
using XOutput.UI.View;

namespace XOutput.UI.Component
{
    /// <summary>
    /// Interaction logic for ControllerView.xaml
    /// </summary>
    public partial class ControllerView : UserControl
    {
        protected readonly ControllerModel viewModel;
        private readonly Action<string> log;

        public ControllerView(GameController controller, Action<string> log = null)
        {
            this.log = log;
            viewModel = new ControllerModel();
            viewModel.Controller = controller;
            viewModel.ButtonText = "Start";
            DataContext = viewModel;
            InitializeComponent();
        }
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var controllerSettingsWindow = new ControllerSettings(new ControllerSettingsViewModel(viewModel.Controller));
            controllerSettingsWindow.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!viewModel.Started)
            {
                int controllerCount = 0;
                controllerCount = viewModel.Controller.Start(() =>
                    {
                        viewModel.ButtonText = "Start";
                        log?.Invoke(string.Format(Message.EmulationStopped, viewModel.Controller.DisplayName));
                        viewModel.Started = false;
                    });
                if (controllerCount != 0)
                {
                    viewModel.ButtonText = "Stop";
                    log?.Invoke(string.Format(Message.EmulationStarted, viewModel.Controller.DisplayName, controllerCount));
                }
                viewModel.Started = controllerCount != 0;
            }
            else
            {
                viewModel.Controller.Stop();
            }
        }
    }
}
