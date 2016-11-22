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
        private const string GameControllersSettings = "joy.cpl";
        private const string SettingsFilePath = "settings.txt";
        private readonly MainWindowViewModel viewModel = new MainWindowViewModel();
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly Devices manager = new Devices();
        private Settings settings;

        public MainWindow()
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        public void Log(string msg)
        {
            Dispatcher.BeginInvoke(new Action(() => logBox.AppendText(msg + Environment.NewLine)));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                settings = Settings.Load(SettingsFilePath);
                Log(string.Format(Message.LoadSettings, SettingsFilePath));
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format(ErrorMessage.LoadSettingsError, SettingsFilePath, ex.Message), ErrorMessage.Warning);
                Log(string.Format(ErrorMessage.LoadSettingsError, SettingsFilePath, ex.Message));
            }
            refreshGameControllers();

            timer.Interval = TimeSpan.FromMilliseconds(10000);
            timer.Tick += (object sender1, EventArgs e1) => { refreshGameControllers(); };
            timer.Start();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            refreshGameControllers();
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
                settings.Save(SettingsFilePath);
                Log(string.Format(Message.SaveSettings, SettingsFilePath));
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(ErrorMessage.SaveSettingsError, SettingsFilePath, ex.Message), ErrorMessage.Warning);
                Log(string.Format(ErrorMessage.SaveSettingsError, SettingsFilePath, ex.Message));
            }
        }

        private void refreshGameControllers()
        {
            List<DirectDevice> devices = manager.GetInputDevices();
            foreach(var controllerView in viewModel.Controllers.ToList())
            {
                var controller = (controllerView.DataContext as ControllerViewModel).Controller;
                if (!devices.Any(x => x.ToString() == controller.DirectInput.ToString()))
                {
                    controller.Dispose();
                    viewModel.Controllers.Remove(controllerView);
                    Log(string.Format(Message.ControllerDisconnected, controller.DisplayName));
                }
            }
            foreach (var device in devices)
            {
                if (!viewModel.Controllers.Any(x => (x.DataContext as ControllerViewModel).Controller.ToString() == device.ToString()))
                {
                    DirectToXInputMapper mapper = settings.GetMapper(device.Id);
                    GameController controller = new GameController(device, mapper);
                    viewModel.Controllers.Add(new ControllerView(controller, Log));
                    Log(string.Format(Message.ControllerConnected, controller.DisplayName));
                }
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Message.AboutContent, Message.About);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            timer.Stop();
            foreach (var controller in viewModel.Controllers.ToList())
            {
                (controller.DataContext as ControllerViewModel).Controller.Dispose();
            }
        }
    }
}
