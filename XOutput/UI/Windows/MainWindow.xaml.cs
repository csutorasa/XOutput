using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using XOutput.Logging;
using XOutput.Tools;

namespace XOutput.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewBase<MainWindowViewModel, MainWindowModel>
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(MainWindow));
        private readonly MainWindowViewModel viewModel;
        public MainWindowViewModel ViewModel => viewModel;
        private bool hardExit = false;
        private WindowState restoreState = WindowState.Normal;

        public MainWindow()
        {
#if DEBUG == false
            Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => viewModel.UnhandledException(e.Exception);
#endif
            viewModel = new MainWindowViewModel(new MainWindowModel(), Dispatcher, Log);
            DataContext = viewModel;
            if (ArgumentParser.Instance.Minimized)
            {
                restoreState = WindowState;
                Visibility = Visibility.Hidden;
            }
            InitializeComponent();
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (ArgumentParser.Instance.Minimized)
            {
                ShowInTaskbar = false;
            }
            viewModel.Initialize();
            await logger.Info("The application has started.");
            await GetData();
        }

        public async Task GetData()
        {

            try
            {
                var result = await new UpdateChecker.UpdateChecker().CompareRelease();
                viewModel.VersionCompare(result);
            }
            catch (Exception) { }
        }

        public void Log(string msg)
        {
            Dispatcher.BeginInvoke((Action)(() => logBox.AppendText(msg + Environment.NewLine)));
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            viewModel.RefreshGameControllers();
        }
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            hardExit = true;
            Close();
        }
        private void GameControllersClick(object sender, RoutedEventArgs e)
        {
            viewModel.OpenWindowsGameControllerSettings();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            viewModel.SaveSettings();
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            viewModel.OpenSettings();
        }

        private void DiagnosticsClick(object sender, RoutedEventArgs e)
        {
            viewModel.OpenDiagnostics();
        }

        private void AboutClick(object sender, RoutedEventArgs e)
        {
            viewModel.AboutPopupShow();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (viewModel.GetSettings().CloseToTray && !hardExit)
            {
                e.Cancel = true;
                restoreState = WindowState;
                Visibility = Visibility.Hidden;
                ShowInTaskbar = false;
            }
        }

        private async void WindowClosed(object sender, EventArgs e)
        {
            viewModel.Finalizer();
            viewModel.Dispose();
            await logger.Info("The application has exited.");
        }

        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            viewModel.RefreshGameControllers();
        }

        private void TaskbarIconTrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                WindowState = restoreState;
            }
            else if (Visibility == Visibility.Hidden)
            {
                ShowInTaskbar = true;
                Visibility = Visibility.Visible;
            }
            Activate();
            Topmost = true;
            Topmost = false;
            Focus();
        }
    }
}
