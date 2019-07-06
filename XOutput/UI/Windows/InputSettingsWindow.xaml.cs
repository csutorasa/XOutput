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
using XOutput.Tools;

namespace XOutput.UI.Windows
{
    /// <summary>
    /// Interaction logic for ControllerSettings.xaml
    /// </summary>
    public partial class InputSettingsWindow : Window, IViewBase<InputSettingsViewModel, InputSettingsModel>
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly InputSettingsViewModel viewModel;
        public InputSettingsViewModel ViewModel => viewModel;
        private readonly IInputDevice device;

        public InputSettingsWindow(InputSettingsViewModel viewModel, IInputDevice device)
        {
            this.device = device;
            this.viewModel = viewModel;
            device.Disconnected += Disconnected;
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
            device.Disconnected -= Disconnected;
            timer.Tick -= TimerTick;
            timer.Stop();
            viewModel.Dispose();
            base.OnClosed(e);
        }

        void Disconnected(object sender, DeviceDisconnectedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Close();
            });
        }

        private void ForceFeedbackButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.TestForceFeedback();
        }

        private void ForceFeedbackCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            viewModel.SetForceFeedbackEnabled();
        }

        private void AddHidGuardianButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.AddHidGuardian();
        }

        private void RemoveHidGuardianButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveHidGuardian();
        }
    }
}
