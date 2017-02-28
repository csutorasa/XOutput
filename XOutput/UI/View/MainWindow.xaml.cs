using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using XOutput.Input;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.UI.Component;
using XOutput.UI.Resources;
using XOutput.UI.View;

namespace XOutput.UI.View
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
            viewModel.RefreshGameControllers();
            viewModel.AddKeyboard();
        }

        public void Log(string msg)
        {
            Dispatcher.BeginInvoke(new Action(() => logBox.AppendText(msg + Environment.NewLine)));
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
            foreach (var controller in viewModel.Model.Controllers.Select(x => (x.DataContext as ControllerModel).Controller))
            {
                controller.Dispose();
            }
        }
    }
}
