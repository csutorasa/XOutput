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
using XOutput.UI.Component;

namespace XOutput.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewBase<MainWindowViewModel, MainWindowModel>
    {
        private readonly MainWindowViewModel viewModel;
        public MainWindowViewModel ViewModel => viewModel;

        public MainWindow()
        {
            this.viewModel = new MainWindowViewModel(new MainWindowModel(), Dispatcher, Log);
            DataContext = viewModel;
            InitializeComponent();
            Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => viewModel.UnhandledException(e.Exception);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
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

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            viewModel.RefreshGameControllers();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void GameControllers_Click(object sender, RoutedEventArgs e)
        {
            viewModel.OpenWindowsGameControllerSettings();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SaveSettings();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AboutPopupShow();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            viewModel.Finalizer();
            viewModel.Dispose();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.RefreshGameControllers();
        }
    }
}
