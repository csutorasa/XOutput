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
using XOutput.UpdateChecker;

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
        private bool installed;

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
                installed = true;
            }
            else
            {
                if (scp)
                {
                    log(Translate("VigemNotInstalled"));
                    installed = true;
                }
                else
                {
                    string error = Translate("VigemAndScpNotInstalled");
                    log(error);
                    installed = false;
                    MessageBox.Show(error, Translate("Error"));
                }
            }
            RefreshGameControllers();

            var controllerView = new ControllerView(new GameController(new Input.Keyboard.Keyboard(), settings.GetMapper("Keyboard")), log);
            controllerView.ViewModel.Model.CanStart = installed;
            Model.Controllers.Add(controllerView);
            log(string.Format(LanguageModel.Instance.Translate("ControllerConnected"), LanguageModel.Instance.Translate("Keyboard")));
        }

        public void Finalizer()
        {
            foreach (var controller in Model.Controllers.Select(x => x.ViewModel.Model.Controller))
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
            MessageBox.Show(Translate("AboutContent") + Environment.NewLine + $"Version {UpdateChecker.Version.AppVersion}", Translate("AboutMenu"));
        }

        public void VersionCompare(VersionCompare compare)
        {
            switch (compare)
            {
                case UpdateChecker.VersionCompare.Error:
                    log(Translate("VersionCheckError"));
                    break;
                case UpdateChecker.VersionCompare.NeedsUpgrade:
                    log(Translate("VersionCheckNeedsUpgrade"));
                    break;
                case UpdateChecker.VersionCompare.NewRelease:
                    log(Translate("VersionCheckNewRelease"));
                    break;
                case UpdateChecker.VersionCompare.UpToDate:
                    log(Translate("VersionCheckUpToDate"));
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public void RefreshGameControllers()
        {
            IEnumerable<DirectDevice> devices = directInputDevices.GetInputDevices(Model.AllDevices);

            foreach (var controllerView in Model.Controllers.ToList())
            {
                var controller = controllerView.ViewModel.Model.Controller;
                if (controller.InputDevice is DirectDevice && !devices.Any(x => x.ToString() == controller.InputDevice.ToString()))
                {
                    controller.Dispose();
                    Model.Controllers.Remove(controllerView);
                    log(string.Format(LanguageModel.Instance.Translate("ControllerDisconnected"), controller.DisplayName));
                }
            }
            foreach (var device in devices)
            {
                if (!Model.Controllers.Any(x => x.ViewModel.Model.Controller.ToString() == device.ToString()))
                {
                    InputMapperBase mapper = settings.GetMapper(device.ToString());
                    GameController controller = new GameController(device, mapper);
                    var controllerView = new ControllerView(controller, log);
                    controllerView.ViewModel.Model.CanStart = installed;
                    Model.Controllers.Add(controllerView);
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
