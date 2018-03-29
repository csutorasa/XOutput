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
using XOutput.UI.Component;

namespace XOutput.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewBase<MainWindowViewModel, MainWindowModel>
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(MainWindow));
        private readonly MainWindowViewModel viewModel;
        public MainWindowViewModel ViewModel => viewModel;

        public MainWindow()
        {
            this.viewModel = new MainWindowViewModel(new MainWindowModel(), Dispatcher, Log);
            DataContext = viewModel;
            InitializeComponent();
#if DEBUG == false
            Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => viewModel.UnhandledException(e.Exception);
#endif
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e)
        {
            viewModel.Initialize();
            foreach (var child in (Content as Grid).Children)
            {
                if (child is Menu)
                {
                    Menu menu = child as Menu;
                    MenuItem langMenu = menu.Items[1] as MenuItem;
                    foreach (var language in LanguageManager.Instance.GetLanguages())
                    {
                        MenuItem langMenuItem = new MenuItem();
                        langMenuItem.Header = language;
                        langMenuItem.Click += (sender1, e1) => { LanguageManager.Instance.Language = language; };
                        langMenu.Items.Add(langMenuItem);
                    }
                }
            }
            logger.Info("The application has started.");
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

        private void AboutClick(object sender, RoutedEventArgs e)
        {
            viewModel.AboutPopupShow();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            viewModel.Finalizer();
            viewModel.Dispose();
            logger.Info("The application has exited.");
        }

        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            viewModel.RefreshGameControllers();
        }
    }
}
