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
using System.Windows.Shapes;
using System.Windows.Threading;
using XOutput.Input;
using XOutput.Input.Mapper;
using XOutput.Input.XInput;

namespace XOutput.UI.View
{
    /// <summary>
    /// Interaction logic for AutoConfigureWindow.xaml
    /// </summary>
    public partial class AutoConfigureWindow : Window, IViewBase<AutoConfigureViewModel, AutoConfigureModel>
    {
        private readonly AutoConfigureViewModel viewModel;
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly bool timed;
        public AutoConfigureViewModel ViewModel => viewModel;

        public AutoConfigureWindow(AutoConfigureViewModel viewModel, bool timed)
        {
            this.viewModel = viewModel;
            this.timed = timed;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.Initialize();
            if (timed)
            {
                timer.Interval = TimeSpan.FromMilliseconds(25);
                timer.Tick += Timer_Tick;
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (viewModel.IncreaseTime())
            {
                if (!viewModel.SaveValues())
                {
                    Close();
                }
            }
        }

        private void Disable_Click(object sender, RoutedEventArgs e)
        {
            if (!viewModel.SaveDisableValues())
            {
                Close();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!viewModel.SaveValues())
            {
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            timer.Tick -= Timer_Tick;
            timer.Stop();
            viewModel.Close();
        }
    }
}
