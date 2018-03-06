using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using XOutput.Input;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.Input.XInput.SCPToolkit;
using XOutput.Input.XInput.Vigem;
using XOutput.UI.Component;

namespace XOutput.UI
{
    public class MainWindowViewModel : ViewModelBase<MainWindowModel>, IDisposable
    {
        private const string SettingsFilePath = "settings.txt";
        private const string GameControllersSettings = "joy.cpl";

        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly Devices directInputDevices = new Devices();
        private readonly Action<string> log;
        private Settings settings;

        public MainWindowViewModel(Action<string> logger)
        {
            model = new MainWindowModel();
            log = logger;
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

        public void Initialize()
        {
            LanguageManager languageManager = LanguageManager.Instance;
            try
            {
                LoadSettings(SettingsFilePath);
                log(string.Format(Translate("LoadSettingsSuccess"), SettingsFilePath));
            }
            catch (Exception ex)
            {
                string error = string.Format(Translate("LoadSettingsError"), SettingsFilePath) + Environment.NewLine + ex.Message;
                log(error);
                MessageBox.Show(error, Translate("Warning"));
            }
            bool vigem = VigemDevice.IsAvailable();
            bool scp = ScpDevice.IsAvailable();
            if (vigem)
            {
                if (scp)
                {
                    log(Translate("ScpInstalled"));
                }
            }
            else
            {
                if (scp)
                {
                    log(Translate("VigemNotInstalled"));
                }
                else
                {
                    string error = Translate("VigemAndScpNotInstalled");
                    log(error);
                    MessageBox.Show(error, Translate("Error"));
                }
            }
            RefreshGameControllers();

            Model.Controllers.Add(new ControllerView(new GameController(new Input.Keyboard.Keyboard(), settings.GetMapper("Keyboard")), log));
            log(string.Format(LanguageModel.Instance.Translate("ControllerConnected"), LanguageModel.Instance.Translate("Keyboard")));
        }

        public void Finalizer()
        {
            foreach (var controller in Model.Controllers.Select(x => (x.DataContext as ControllerViewModel).Model.Controller))
            {
                controller.Dispose();
            }
        }
        
        public void SaveSettings()
        {
            try
            {
                settings.Save(SettingsFilePath);
                log(string.Format(Translate("SaveSettingsSuccess"), SettingsFilePath));
            }
            catch (Exception ex)
            {
                string error = string.Format(Translate("SaveSettingsError"), SettingsFilePath) + Environment.NewLine + ex.Message;
                log(error);
                MessageBox.Show(error, Translate("Warning"));
            }
        }

        public void AboutPopupShow()
        {
            MessageBox.Show(Translate("AboutContent"), Translate("AboutMenu"));
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
                    log(string.Format(LanguageModel.Instance.Translate("ControllerDisconnected"), controller.DisplayName));
                }
            }
            foreach (var device in devices)
            {
                if (!Model.Controllers.Any(x => (x.DataContext as ControllerViewModel).Model.Controller.ToString() == device.ToString()))
                {
                    InputMapperBase mapper = settings.GetMapper(device.ToString());
                    GameController controller = new GameController(device, mapper);
                    Model.Controllers.Add(new ControllerView(controller, log));
                    device.StartCapturing();
                    log(string.Format(LanguageModel.Instance.Translate("ControllerConnected"), controller.DisplayName));
                }
                else
                {
                    device.Dispose();
                }
            }
        }

        public void OpenWindowsGameControllerSettings()
        {
            Process.Start(GameControllersSettings);
        }

        private string Translate(string key)
        {
            return LanguageModel.Instance.Translate(key);
        }
    }
}
