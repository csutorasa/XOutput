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
using XOutput.Devices;
using XOutput.Devices.Input;
using XOutput.Devices.Input.DirectInput;
using XOutput.Devices.Mapper;
using XOutput.Devices.XInput.SCPToolkit;
using XOutput.Devices.XInput.Vigem;
using XOutput.Diagnostics;
using XOutput.Logging;
using XOutput.Tools;
using XOutput.UI.Component;
using XOutput.UpdateChecker;

namespace XOutput.UI.Windows
{
    public class MainWindowViewModel : ViewModelBase<MainWindowModel>, IDisposable
    {
        private readonly int pid = Process.GetCurrentProcess().Id;
        private const string SettingsFilePath = "settings.json";
        private const string GameControllersSettings = "joy.cpl";

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(MainWindowViewModel));
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly DirectInputDevices directInputDevices = new DirectInputDevices();
        private Action<string> log;
        private readonly Dispatcher dispatcher;
        private Settings settings;
        private bool installed;

        public MainWindowViewModel(MainWindowModel model, Dispatcher dispatcher) : base(model)
        {
            this.dispatcher = dispatcher;
            timer.Interval = TimeSpan.FromMilliseconds(10000);
            timer.Tick += (object sender1, EventArgs e1) => { RefreshGameControllers(); };
            timer.Start();
        }

        public async void UnhandledException(Exception exceptionObject)
        {
            await logger.Error(exceptionObject);
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

        public void Initialize(Action<string> log)
        {
            this.log = log;
            LanguageManager languageManager = LanguageManager.Instance;
            try
            {
                LoadSettings(SettingsFilePath);
                languageManager.Language = settings.Language;
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
            if (settings.HidGuardianEnabled)
            {
                try
                {
                    HidGuardianManager.Instance.ResetPid(pid);
                    Model.IsAdmin = true;
                    logger.Info("HidGuardian registry is set");
                    log(string.Format(Translate("HidGuardianEnabledSuccessfully"), pid.ToString()));
                }
                catch (UnauthorizedAccessException)
                {
                    Model.IsAdmin = false;
                    logger.Warning("Not running in elevated mode.");
                    log(Translate("HidGuardianNotAdmin"));
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(ex.ToString());
                }
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
            Model.Settings = settings;
            RefreshGameControllers();

            logger.Debug("Creating keyboard controller");
            Devices.Input.Keyboard.Keyboard keyboard = new Devices.Input.Keyboard.Keyboard();
            InputConfig inputConfig = settings.GetInputConfiguration(keyboard.ToString(), keyboard.InputConfiguration);
            InputDevices.Instance.Add(keyboard);
            Model.Inputs.Add(new InputView(new InputViewModel(new InputModel(), keyboard, false)));
            foreach (var mapping in settings.Mapping)
            {
                AddController(mapping);
            }
            log(string.Format(LanguageModel.Instance.Translate("ControllerConnected"), LanguageModel.Instance.Translate("Keyboard")));
            logger.Info("Keyboard controller is connected");
            HandleArgs();
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
                logger.Warning(ex);
                string error = string.Format(Translate("SaveSettingsError"), SettingsFilePath) + Environment.NewLine + ex.Message;
                log(error);
                MessageBox.Show(error, Translate("Warning"));
            }
        }

        public void AboutPopupShow()
        {
            MessageBox.Show(Translate("AboutContent") + Environment.NewLine + string.Format(Translate("Version"), UpdateChecker.Version.AppVersion), Translate("AboutMenu"));
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

            foreach (var inputView in Model.Inputs.ToArray())
            {
                var device = inputView.ViewModel.Model.Device;
                if (device is DirectDevice && (!instances.Any(x => x.InstanceGuid == ((DirectDevice)device).Id) || !device.Connected))
                {
                    Model.Inputs.Remove(inputView);
                    inputView.ViewModel.Dispose();
                    device.Dispose();
                    InputDevices.Instance.Remove(device);
                }
            }
            foreach (var instance in instances)
            {
                if (!Model.Inputs.Select(c => c.ViewModel.Model.Device).OfType<DirectDevice>().Any(d => d.Id == instance.InstanceGuid))
                {
                    var device = directInputDevices.CreateDirectDevice(instance);
                    if (device == null)
                        continue;
                    InputMapper mapper = settings.GetMapper(device.ToString());
                    InputConfig inputConfig = settings.GetInputConfiguration(device.ToString(), device.InputConfiguration);
                    device.Disconnected -= DispatchRefreshGameControllers;
                    device.Disconnected += DispatchRefreshGameControllers;
                    Model.Inputs.Add(new InputView(new InputViewModel(new InputModel(), device, Model.IsAdmin)));
                }
            }
        }

        public void AddController(InputMapper mapper)
        {
            var gameController = new GameController(mapper == null ? settings.CreateMapper(Guid.NewGuid().ToString()) : mapper);
            Controllers.Instance.Add(gameController);

            var controllerView = new ControllerView(new ControllerViewModel(new ControllerModel(), gameController, Model.IsAdmin, log));
            controllerView.ViewModel.Model.CanStart = installed;
            controllerView.RemoveClicked += RemoveController;
            Model.Controllers.Add(controllerView);
            log(string.Format(LanguageModel.Instance.Translate("ControllerConnected"), gameController.DisplayName));
            if (mapper?.StartWhenConnected == true)
            {
                controllerView.ViewModel.Start();
                logger.Info($"{mapper.Name} controller is started automatically.");
            }
        }

        public void RemoveController(ControllerView controllerView)
        {
            var controller = controllerView.ViewModel.Model.Controller;
            controllerView.ViewModel.Dispose();
            controller.Dispose();
            Model.Controllers.Remove(controllerView);
            logger.Info($"{controller.ToString()} is disconnected.");
            log(string.Format(LanguageModel.Instance.Translate("ControllerDisconnected"), controller.DisplayName));
            Controllers.Instance.Remove(controller);
            settings.Mapping.RemoveAll(m => m.Id == controller.Mapper.Id);
        }

        public void OpenWindowsGameControllerSettings()
        {
            logger.Debug("Starting " + GameControllersSettings);
            Process.Start(GameControllersSettings);
            logger.Debug("Started " + GameControllersSettings);
        }

        public void OpenSettings()
        {
            new SettingsWindow(new SettingsViewModel(new SettingsModel(settings))).ShowDialog();
        }

        public void OpenDiagnostics()
        {
            IList<IDiagnostics> elements = InputDevices.Instance.GetDevices()
                .Select(d => new InputDiagnostics(d)).OfType<IDiagnostics>().ToList();
            elements.Insert(0, new Devices.XInput.XInputDiagnostics());

            new DiagnosticsWindow(new DiagnosticsViewModel(new DiagnosticsModel(), elements)).ShowDialog();
        }

        private string Translate(string key)
        {
            return LanguageModel.Instance.Translate(key);
        }

        private void DispatchRefreshGameControllers(object sender, DeviceDisconnectedEventArgs e)
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

        private void HandleArgs()
        {
            foreach (var viewModel in Model.Controllers.Select(v => v.ViewModel).OrderBy(v => v.Model.Controller.DisplayName).ToArray())
            {
                var displayName = viewModel.Model.DisplayName;
                foreach (var startupController in ArgumentParser.Instance.StartControllers)
                {
                    if (displayName.Contains(startupController))
                    {
                        viewModel.Start();
                        logger.Info($"{startupController} controller is started automatically");
                        break;
                    }
                }
            }
        }
    }
}
