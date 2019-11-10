using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace XOutput.UI.Windows
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

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(100);
            viewModel.Initialize();
            viewModel.IsMouseOverButtons = () =>
            {
                return DisableButton.IsMouseOver || SaveButton.IsMouseOver;
            };
            if (timed)
            {
                timer.Interval = TimeSpan.FromMilliseconds(25);
                timer.Tick += TimerTick;
                timer.Start();
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (viewModel.IncreaseTime())
            {
                bool hasNextInput = viewModel.SaveValues();
                if (!hasNextInput)
                {
                    Close();
                }
            }
        }

        private void DisableClick(object sender, RoutedEventArgs e)
        {
            if (!viewModel.SaveDisableValues())
            {
                Close();
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            if (!viewModel.SaveValues())
            {
                Close();
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            timer.Tick -= TimerTick;
            timer.Stop();
            viewModel.Close();
        }
    }
}
