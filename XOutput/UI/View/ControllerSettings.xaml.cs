using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using XOutput.Input;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.Input.XInput;
using XOutput.UI.Component;
using XOutput.UI.View;

namespace XOutput.UI.View
{
    /// <summary>
    /// Interaction logic for ControllerSettings.xaml
    /// </summary>
    public partial class ControllerSettings : Window, IViewBase<ControllerSettingsViewModel, ControllerSettingsModel>
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly ControllerSettingsViewModel viewModel;
        public ControllerSettingsViewModel ViewModel => viewModel;
        private readonly GameController controller;

        public ControllerSettings(GameController controller)
        {
            this.controller = controller;
            viewModel = new ControllerSettingsViewModel(controller);
            controller.InputDevice.Disconnected += Disconnected;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Update();

            timer.Interval = TimeSpan.FromMilliseconds(25);
            timer.Tick += (sender1, e1) => { Update(); };
            timer.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            controller.InputDevice.Disconnected -= Disconnected;
            timer.Stop();
        }

        private void Update()
        {
            if (!viewModel.IsConrtollerConnected)
            {
                return;
            }

            viewModel.UpdateInputControls();

            viewModel.UpdateXInputControls();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ConfigureAll();
        }

        void Disconnected()
        {
            Dispatcher.Invoke(() =>
            {
                Close();
            });
        }
    }
}
