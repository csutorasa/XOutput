using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using XOutput.Input;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.UI.Component;
using XOutput.UI.Resources;

namespace XOutput.UI
{
    public class MainWindowViewModel : ViewModelBase<MainWindowModel>, IDisposable
    {
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly Devices directInputDevices = new Devices();
        private readonly Action<string> logger;
        private Settings settings;

        public MainWindowViewModel(Action<string> logger)
        {
            model = new MainWindowModel();
            this.logger = logger;
            timer.Interval = TimeSpan.FromMilliseconds(10000);
            timer.Tick += (object sender1, EventArgs e1) => { RefreshGameControllers(); };
            timer.Start();
        }

        ~MainWindowViewModel()
        {
            Dispose();
        }

        public void Dispose()
        {
            timer.Stop();
            directInputDevices.Dispose();
        }

        public void LoadSettings(string settingsFilePath)
        {
            try
            {
                settings = Settings.Load(settingsFilePath);
            }
            catch
            {
                settings = new Settings();
                throw;
            }
        }

        public void AddKeyboard()
        {
            Model.Controllers.Add(new ControllerView(new GameController(new Input.Keyboard.Keyboard(), settings.GetMapper("Keyboard"))));
            logger(string.Format(Message.ControllerConnected, "Keyboard"));
        }
        public void SaveSettings(string settingsFilePath)
        {
            settings.Save(settingsFilePath);
        }

        public void RefreshGameControllers()
        {
            IEnumerable<DirectDevice> devices = directInputDevices.GetInputDevices();

            foreach (var controllerView in Model.Controllers.ToList())
            {
                var controller = (controllerView.DataContext as ControllerViewModel).Model.Controller;
                if (controller.InputDevice is DirectDevice && !devices.Any(x => x.ToString() == controller.InputDevice.ToString()))
                {
                    controller.Dispose();
                    Model.Controllers.Remove(controllerView);
                    logger(string.Format(Message.ControllerDisconnected, controller.DisplayName));
                }
            }
            foreach (var device in devices)
            {
                if (!Model.Controllers.Any(x => (x.DataContext as ControllerViewModel).Model.Controller.ToString() == device.ToString()))
                {
                    InputMapperBase mapper = settings.GetMapper(device.ToString());
                    GameController controller = new GameController(device, mapper);
                    Model.Controllers.Add(new ControllerView(controller, logger));
                    device.StartCapturing();
                    logger(string.Format(Message.ControllerConnected, controller.DisplayName));
                }
                else
                {
                    device.Dispose();
                }
            }
        }
    }
}
