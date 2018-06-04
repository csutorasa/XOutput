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
using XOutput.Devices;
using XOutput.Devices.Input;

namespace XOutput.UI.Windows
{
    /// <summary>
    /// Interaction logic for ControllerSettings.xaml
    /// </summary>
    public partial class ControllerSettingsWindow : Window, IViewBase<ControllerSettingsViewModel, ControllerSettingsModel>
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly ControllerSettingsViewModel viewModel;
        public ControllerSettingsViewModel ViewModel => viewModel;
        private readonly GameController controller;

        public ControllerSettingsWindow(ControllerSettingsViewModel viewModel, GameController controller)
        {
            this.controller = controller;
            this.viewModel = viewModel;
            //controller.InputDevice.Disconnected += Disconnected;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            viewModel.Update();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            viewModel.Update();
        }

        protected override void OnClosed(EventArgs e)
        {
            //controller.InputDevice.Disconnected -= Disconnected;
            timer.Tick -= TimerTick;
            timer.Stop();
            viewModel.Dispose();
            base.OnClosed(e);
        }

        private void ConfigureAllButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.ConfigureAll();
        }

        void Disconnected(object sender, DeviceDisconnectedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Close();
            });
        }

        private void ComboBoxSelected(object sender, RoutedEventArgs e)
        {
            viewModel.SelectedDPad();
        }

        private void ForceFeedbackButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.TestForceFeedback();
        }

        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            viewModel.SetStartWhenConnected();
        }
    }
}
