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

        public MainWindow(MainWindowViewModel viewModel)
        {
#if DEBUG == false
            Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => viewModel.UnhandledException(e.Exception);
#endif
            this.viewModel = viewModel;
            DataContext = viewModel;
            if (ArgumentParser.Instance.Minimized)
            {
                Visibility = Visibility.Hidden;
                ShowInTaskbar = false;
                logger.Info("Starting XOutput in minimized to taskbar");
            }
            else
            {
                ShowInTaskbar = true;
                logger.Info("Starting XOutput in normal window");
            }
            new WindowInteropHelper(this).EnsureHandle();
            InitializeComponent();
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            viewModel.Initialize(Log);
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
            Dispatcher.BeginInvoke((Action)(() =>
            {
                try
                {
                    logBox.AppendText(msg + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    logger.Error("Cannot log into the log box: " + msg + Environment.NewLine);
                    logger.Error(ex);
                }
            }));
        }

        private void AddControllerClick(object sender, RoutedEventArgs e)
        {
            viewModel.AddController(null);
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            viewModel.RefreshGameControllers();
        }
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            hardExit = true;
            if (IsLoaded)
            {
                Close();
            }
            else
            {
                viewModel.Finalizer();
                viewModel.Dispose();
                logger.Info("The application will exit.");
                Environment.Exit(0);
            }

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
                logger.Info("The application is closed to tray.");
            }
        }

        private async void WindowClosed(object sender, EventArgs e)
        {
            viewModel.Finalizer();
            viewModel.Dispose();
            await logger.Info("The application will exit.");
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
                if (!IsLoaded)
                {
                    Show();
                }
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
