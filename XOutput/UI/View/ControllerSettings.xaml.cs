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
    public partial class ControllerSettings : Window
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly ControllerSettingsViewModel viewModel;

        public ControllerSettings(ControllerSettingsViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            update();
            
            timer.Interval = TimeSpan.FromMilliseconds(25);
            timer.Tick += (sender1, e1) => { update(); };
            timer.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void update()
        {
            if (!viewModel.IsConrtollerConnected)
            {
                Close();
                return;
            }

            viewModel.updateInputControls();

            viewModel.updateXInputControls();
        }
    }
}
