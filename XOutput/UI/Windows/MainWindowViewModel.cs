using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Threading;
using XOutput.Devices;
using XOutput.Devices.Input;
using XOutput.Devices.Input.DirectInput;
using XOutput.Devices.Mapper;
using XOutput.Devices.XInput;
using XOutput.Diagnostics;
using XOutput.Tools;
using XOutput.UI.Component;
using XOutput.Versioning;

namespace XOutput.UI.Windows
{
    public class MainWindowViewModel : ViewModelBase<MainWindowModel>, IDisposable
    {
        private readonly int pid = Process.GetCurrentProcess().Id;
        private const string SettingsFilePath = "settings.json";
        private const string GameControllersSettings = "joy.cpl";

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly Dispatcher dispatcher;
        private readonly UpdateChecker updateChecker;
        private readonly HidGuardianManager hidGuardianManager;
        private readonly XOutputManager xOutputManager;

        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly DirectInputDevices directInputDevices = new DirectInputDevices();
        private Action<string> log;
        private Settings settings;

        public MainWindowViewModel(MainWindowModel model, Dispatcher dispatcher,
            UpdateChecker updateChecker, HidGuardianManager hidGuardianManager, XOutputManager xOutputManager) : base(model)
        {
            this.dispatcher = dispatcher;
            this.updateChecker = updateChecker;
            this.hidGuardianManager = hidGuardianManager;
            this.xOutputManager = xOutputManager;
            timer.Interval = TimeSpan.FromMilliseconds(10000);
            timer.Tick += (object sender1, EventArgs e1) => { RefreshGameControllers(); };
            timer.Start();
        }

        public void Dispose()
        {
            foreach (var device in Model.Inputs.Select(x => x.ViewModel.Model.Device))
            {
                device.Dispose();
            }
            foreach (var controller in Model.Controllers.Select(x => x.ViewModel.Model.Controller))
            {
                controller.Dispose();
            }
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
                logger.Warn("Loading settings was unsuccessful.");
                string error = string.Format(Translate("LoadSettingsError"), SettingsFilePath) + Environment.NewLine + ex.Message;
                log(error);
                MessageBox.Show(error, Translate("Warning"));
            }
            if (settings.HidGuardianEnabled)
            {
                try
                {
                    hidGuardianManager.ResetPid(pid);
                    Model.IsAdmin = true;
                    logger.Info("HidGuardian registry is set");
                    log(string.Format(Translate("HidGuardianEnabledSuccessfully"), pid.ToString()));
                }
                catch (UnauthorizedAccessException)
                {
                    Model.IsAdmin = false;
                    logger.Warn("Not running in elevated mode.");
                    log(Translate("HidGuardianNotAdmin"));
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    MessageBox.Show(ex.ToString());
                }
            }
            if (xOutputManager.IsVigem)
            {
                if (xOutputManager.IsScp)
                {
                    logger.Info("SCPToolkit is obsolete.");
                    log(Translate("ScpInstalled"));
                }
                else
                {
                    logger.Info("ViGEm is installed.");
                }
            }
            else if (xOutputManager.IsScp)
            {
                logger.Info("ScpToolkit is installed only.");
                log(Translate("VigemNotInstalled"));
            }
            else
            {
                logger.Error("Neither ViGEm nor SCPToolkit is installed.");
                string error = Translate("VigemAndScpNotInstalled");
                log(error);
                MessageBox.Show(error, Translate("Error"));
            }
            Model.Settings = settings;
            RefreshGameControllers();

            logger.Debug("Creating keyboard controller");
            Devices.Input.Keyboard.Keyboard keyboard = new Devices.Input.Keyboard.Keyboard();
            settings.GetOrCreateInputConfiguration(keyboard.ToString(), keyboard.InputConfiguration);
            InputDevices.Instance.Add(keyboard);
            Model.Inputs.Add(new InputView(new InputViewModel(new InputModel(), keyboard, false)));
            logger.Debug("Creating mouse controller");
            Devices.Input.Mouse.Mouse mouse = new Devices.Input.Mouse.Mouse();
            settings.GetOrCreateInputConfiguration(mouse.ToString(), mouse.InputConfiguration);
            InputDevices.Instance.Add(mouse);
            Model.Inputs.Add(new InputView(new InputViewModel(new InputModel(), mouse, false)));
            foreach (var mapping in settings.Mapping)
            {
                AddController(mapping);
            }
            log(string.Format(LanguageModel.Instance.Translate("ControllerConnected"), LanguageModel.Instance.Translate("Keyboard")));
            logger.Info("Keyboard controller is connected");
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
                logger.Warn(ex, "Saving settings was unsuccessful.");
                string error = string.Format(Translate("SaveSettingsError"), SettingsFilePath) + Environment.NewLine + ex.Message;
                log(error);
                MessageBox.Show(error, Translate("Warning"));
            }
        }

        public void AboutPopupShow()
        {
            MessageBox.Show(Translate("AboutContent") + Environment.NewLine + string.Format(Translate("Version"), Versioning.Version.AppVersion), Translate("AboutMenu"));
        }

        public async Task CompareVersion()
        {
            var result = await updateChecker.CompareRelease();
            switch (result)
            {
                case VersionCompare.Error:
                    logger.Warn("Failed to check latest version");
                    log(Translate("VersionCheckError"));
                    break;
                case VersionCompare.NeedsUpgrade:
                    logger.Info("New version is available");
                    log(Translate("VersionCheckNeedsUpgrade"));
                    break;
                case VersionCompare.NewRelease:
                    log(Translate("VersionCheckNewRelease"));
                    break;
                case VersionCompare.UpToDate:
                    logger.Info("Version is up-to-date");
                    log(Translate("VersionCheckUpToDate"));
                    break;
                default:
                    throw new ArgumentException(nameof(result));
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
                    InputDevices.Instance.Remove(device);
                    inputView.ViewModel.Dispose();
                    device.Dispose();
                }
            }
            foreach (var instance in instances)
            {
                if (!Model.Inputs.Select(c => c.ViewModel.Model.Device).OfType<DirectDevice>().Any(d => d.Id == instance.InstanceGuid))
                {
                    var device = directInputDevices.CreateDirectDevice(instance);
                    if (device == null)
                    {
                        continue;
                    }
                    InputConfig inputConfig = settings.GetOrCreateInputConfiguration(device.ToString(), device.InputConfiguration);
                    device.Disconnected -= DispatchRefreshGameControllers;
                    device.Disconnected += DispatchRefreshGameControllers;
                    Model.Inputs.Add(new InputView(new InputViewModel(new InputModel(), device, Model.IsAdmin)));
                }
            }
        }

        public void AddController(InputMapper mapper)
        {
            var gameController = new GameController(mapper ?? settings.CreateMapper(Guid.NewGuid().ToString()));
            Controllers.Instance.Add(gameController);

            var controllerView = new ControllerView(new ControllerViewModel(new ControllerModel(), gameController, Model.IsAdmin, log));
            controllerView.ViewModel.Model.CanStart = xOutputManager.HasDevice;
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
            new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C " + GameControllersSettings,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                },
            }.Start();
            logger.Debug("Started " + GameControllersSettings);
        }

        public void OpenSettings()
        {
            ApplicationContext context = ApplicationContext.Global.WithSingletons(settings);
            SettingsWindow settingsWindow = context.Resolve<SettingsWindow>();
            settingsWindow.ShowDialog();
        }

        public void OpenDiagnostics()
        {
            IList<IDiagnostics> elements = InputDevices.Instance.GetDevices()
                .Select(d => new InputDiagnostics(d)).OfType<IDiagnostics>().ToList();
            elements.Insert(0, new Devices.XInput.XInputDiagnostics());

            ApplicationContext context = ApplicationContext.Global.WithSingletons(new DiagnosticsModel(elements));
            DiagnosticsWindow diagnosticsWindow = context.Resolve<DiagnosticsWindow>();
            diagnosticsWindow.ShowDialog();
        }

        private string Translate(string key)
        {
            return LanguageModel.Instance.Translate(key);
        }

        private void DispatchRefreshGameControllers(object sender, DeviceDisconnectedEventArgs e)
        {
            ThreadCreator.Create("Device list refresh delay", (token) =>
            {
                Thread.Sleep(1000);
                dispatcher.Invoke(RefreshGameControllers);
            }).Start();
        }
    }
}
