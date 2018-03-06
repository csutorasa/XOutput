using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using XOutput.UI.Component;
using XOutput.UI.Resources;

namespace XOutput.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel viewModel;
        private const string SettingsFilePath = "settings.txt";
        private const string GameControllersSettings = "joy.cpl";

        public MainWindow()
        {
            viewModel = new MainWindowViewModel(Log);
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LanguageManager languageManager = LanguageManager.getInstance();
            foreach (var child in (Content as Grid).Children)
            {
                if(child is Menu)
                {
                    Menu menu = child as Menu;
                    MenuItem langMenu = menu.Items[1] as MenuItem;
                    foreach (var language in languageManager.GetLanguages()) {
                        MenuItem langMenuItem = new MenuItem();
                        langMenuItem.Header = language;
                        langMenuItem.Click += (sender1, e1) => { languageManager.Language = language; };
                        langMenu.Items.Add(langMenuItem);
                    }
                }
            }
            try
            {
                viewModel.LoadSettings(SettingsFilePath);
                Log(string.Format(Message.LoadSettings, SettingsFilePath));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(ErrorMessage.LoadSettingsError, SettingsFilePath, ex.Message), ErrorMessage.Warning);
                Log(string.Format(ErrorMessage.LoadSettingsError, SettingsFilePath, ex.Message));
            }
            try
            {
                viewModel.RefreshGameControllers();
                viewModel.AddKeyboard();
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format(ErrorMessage.SCPNotInstalledError, ex.Message), ErrorMessage.Warning);
                Log(string.Format(Message.ScpDownload));
            }
        }

        public void Log(string msg)
        {
            Dispatcher.BeginInvoke(new Action(() => logBox.AppendText(msg + Environment.NewLine)));
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel.RefreshGameControllers();
            }
            catch (IOException) { }
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void GameControllers_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(GameControllersSettings);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel.SaveSettings(SettingsFilePath);
                Log(string.Format(Message.SaveSettings, SettingsFilePath));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(ErrorMessage.SaveSettingsError, SettingsFilePath, ex.Message), ErrorMessage.Warning);
                Log(string.Format(ErrorMessage.SaveSettingsError, SettingsFilePath, ex.Message));
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Message.AboutContent, Message.About);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            viewModel.Dispose();
            foreach (var controller in viewModel.Model.Controllers.Select(x => (x.DataContext as ControllerViewModel).Model.Controller))
            {
                controller.Dispose();
            }
        }
    }
}
