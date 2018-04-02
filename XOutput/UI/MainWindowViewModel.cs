using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using XOutput.Input;
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.Input.XInput.SCPToolkit;
using XOutput.Input.XInput.Vigem;
using XOutput.Logging;
using XOutput.UI.Component;
using XOutput.UI.View;
using XOutput.UpdateChecker;

namespace XOutput.UI
{
    public class MainWindowViewModel : ViewModelBase<MainWindowModel>, IDisposable
    {
        private const string SettingsFilePath = "settings.txt";
        private const string GameControllersSettings = "joy.cpl";

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(MainWindowViewModel));
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly Devices directInputDevices = new Devices();
        private readonly Action<string> log;
        private readonly Dispatcher dispatcher;
        private Settings settings;
        private bool installed;

        public MainWindowViewModel(MainWindowModel model, Dispatcher dispatcher, Action<string> logger) : base(model)
        {
            log = logger;
            this.dispatcher = dispatcher;
            timer.Interval = TimeSpan.FromMilliseconds(10000);
            timer.Tick += (object sender1, EventArgs e1) => { RefreshGameControllers(); };
            timer.Start();
        }

        public void UnhandledException(Exception exceptionObject)
        {
            logger.Error(exceptionObject);
            MessageBox.Show(exceptionObject.Message + Environment.NewLine + exceptionObject.StackTrace);
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

        public Settings GetSettings()
        {
            return settings;
        }

        public void Initialize()
        {
            LanguageManager languageManager = LanguageManager.Instance;
            try
            {
                LoadSettings(SettingsFilePath);
                logger.Info("Loading settings was successful.");
                log(string.Format(Translate("LoadSettingsSuccess"), SettingsFilePath));
            }
            catch (Exception ex)
            {
                logger.Warning("Loading settings was unsuccessful.");
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
                    logger.Info("SCPToolkit is installed only.");
                    log(Translate("ScpInstalled"));
                }
                installed = true;
            }
            else
            {
                if (scp)
                {
                    logger.Info("ViGEm is installed.");
                    log(Translate("VigemNotInstalled"));
                    installed = true;
                }
                else
                {
                    logger.Error("Neither ViGEm nor SCPToolkit is installed.");
                    string error = Translate("VigemAndScpNotInstalled");
                    log(error);
                    installed = false;
                    MessageBox.Show(error, Translate("Error"));
                }
            }
            RefreshGameControllers();

            var keyboardGameController = new GameController(new Input.Keyboard.Keyboard(), settings.GetMapper("Keyboard"));
            var controllerView = new ControllerView(new ControllerViewModel(new ControllerModel(), keyboardGameController, log));
            controllerView.ViewModel.Model.CanStart = installed;
            Model.Controllers.Add(controllerView);
            log(string.Format(LanguageModel.Instance.Translate("ControllerConnected"), LanguageModel.Instance.Translate("Keyboard")));

            AutoStart();
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
                logger.Info("Saving settings was successful.");
                log(string.Format(Translate("SaveSettingsSuccess"), SettingsFilePath));
            }
            catch (Exception ex)
            {
                logger.Warning("Saving settings was unsuccessful.");
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
                    logger.Warning("Failed to check latest version");
                    log(Translate("VersionCheckError"));
                    break;
                case UpdateChecker.VersionCompare.NeedsUpgrade:
                    logger.Info("New version is available");
                    log(Translate("VersionCheckNeedsUpgrade"));
                    break;
                case UpdateChecker.VersionCompare.NewRelease:
                    log(Translate("VersionCheckNewRelease"));
                    break;
                case UpdateChecker.VersionCompare.UpToDate:
                    logger.Info("Version is up-to-date");
                    log(Translate("VersionCheckUpToDate"));
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public void RefreshGameControllers()
        {
            IEnumerable<SharpDX.DirectInput.DeviceInstance> instances = directInputDevices.GetInputDevices(Model.AllDevices);

            foreach (var controllerView in Model.Controllers.ToList())
            {
                var controller = controllerView.ViewModel.Model.Controller;
                if (controller.InputDevice is DirectDevice && (!instances.Any(x => x.InstanceGuid == ((DirectDevice)controller.InputDevice).Id) || !controller.InputDevice.Connected))
                {
                    controller.Dispose();
                    Model.Controllers.Remove(controllerView);
                    logger.Info($"{controller.DisplayName} was disconnected.");
                    log(string.Format(LanguageModel.Instance.Translate("ControllerDisconnected"), controller.DisplayName));
                }
            }
            foreach (var instance in instances)
            {
                if (!Model.Controllers.Select(c => c.ViewModel.Model.Controller.InputDevice).OfType<DirectDevice>().Any(d => d.Id == instance.InstanceGuid))
                {
                    var device = directInputDevices.CreateDirectDevice(instance);
                    if (device == null)
                        continue;
                    InputMapperBase mapper = settings.GetMapper(device.ToString());
                    GameController controller = new GameController(device, mapper);
                    var controllerView = new ControllerView(new ControllerViewModel(new ControllerModel(), controller, log));
                    controllerView.ViewModel.Model.CanStart = installed;
                    Model.Controllers.Add(controllerView);
                    device.StartCapturing();
                    device.Disconnected -= DispatchRefreshGameControllers;
                    device.Disconnected += DispatchRefreshGameControllers;
                    logger.Info($"{controller.DisplayName} was connected.");
                    log(string.Format(LanguageModel.Instance.Translate("ControllerConnected"), controller.DisplayName));
                }
            }
        }

        public void OpenWindowsGameControllerSettings()
        {
            logger.Debug("Starting " + GameControllersSettings);
            Process.Start(GameControllersSettings);
        }

        public void OpenSettings()
        {
            new SettingsWindow(new SettingsViewModel(new SettingsModel(settings))).ShowDialog();
        }

        private string Translate(string key)
        {
            return LanguageModel.Instance.Translate(key);
        }

        private void DispatchRefreshGameControllers()
        {
            Thread delayThread = new Thread(() =>
            {
                Thread.Sleep(1000);
                dispatcher.Invoke(RefreshGameControllers);
            });
            delayThread.Name = "Device list refresh delay";
            delayThread.IsBackground = true;
            delayThread.Start();
        }

        private void AutoStart()
        {
            var args = Environment.GetCommandLineArgs();
            var startupControllers = args.Where(arg => arg.StartsWith("--start=")).Select(arg => arg.Replace("--start=", "")).ToArray();
            foreach (var viewModel in Model.Controllers.Select(v => v.ViewModel))
            {
                var displayName = viewModel.Model.DisplayName;
                foreach (var startupController in startupControllers)
                {
                    if (displayName.Contains(startupController))
                    {
                        viewModel.StartStop();
                        break;
                    }
                }
            }
        }
    }
}
