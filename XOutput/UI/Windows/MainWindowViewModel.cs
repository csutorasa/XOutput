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
using XOutput.Devices.Input.Diagnostics;
using XOutput.Devices.Input.DirectInput;
using XOutput.Devices.Input.Keyboard;
using XOutput.Devices.XInput.Diagnostics;
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
        private const string SettingsFilePath = "settings.json";
        private const string GameControllersSettings = "joy.cpl";

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(MainWindowViewModel));
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly DirectInputDevices directInputDevices = new DirectInputDevices();
        private readonly Dispatcher dispatcher;
        private Settings settings;
        private bool installed;

        public MainWindowViewModel(MainWindowModel model, Dispatcher dispatcher) : base(model)
        {
            this.dispatcher = dispatcher;
            directInputDevices.DeviceConnected += DirectInputDevices_DeviceConnected;
            timer.Interval = TimeSpan.FromMilliseconds(10000);
            timer.Tick += (object sender1, EventArgs e1) => { RefreshGameControllers(); };
            timer.Start();
        }

        private void DirectInputDevices_DeviceConnected(object sender, DeviceConnectedEventArgs e)
        {
            var device = sender as DirectDevice;
            var s = settings.GetDeviceSettings(device.Id.ToString());
            GameController controller = new GameController(/*s.Mapping*/ null, /*s.DPadSettings*/ null, /*s.ForceFeedbackDevices*/ null);
            var controllerView = new ControllerView(new ControllerViewModel(new ControllerModel(), controller));
            controllerView.ViewModel.Model.CanStart = installed;
            Model.Controllers.Add(controllerView);
            device.Disconnected -= DispatchRefreshGameControllers;
            device.Disconnected += DispatchRefreshGameControllers;
            logger.Info($"{controller.ToString()} is connected.");
            if (s.StartWhenConnected)
            {
                controllerView.ViewModel.Start();
                logger.Info($"{controller.ToString()} controller is started automatically.");
            }
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

        public void Initialize()
        {
            LanguageManager languageManager = LanguageManager.Instance;
            try
            {
                LoadSettings(SettingsFilePath);
                logger.Info("Loading settings was successful.");
            }
            catch (Exception ex)
            {
                logger.Warning("Loading settings was unsuccessful.");
                string error = string.Format(Translate("LoadSettingsError"), SettingsFilePath) + Environment.NewLine + ex.Message;
                MessageBox.Show(error, Translate("Warning"));
            }
            bool vigem = VigemDevice.IsAvailable();
            bool scp = ScpDevice.IsAvailable();
            if (vigem)
            {
                if (scp)
                {
                    logger.Info("SCPToolkit is installed only.");
                }
                installed = true;
            }
            else
            {
                if (scp)
                {
                    logger.Info("ViGEm is installed.");
                    installed = true;
                }
                else
                {
                    logger.Error("Neither ViGEm nor SCPToolkit is installed.");
                    string error = Translate("VigemAndScpNotInstalled");
                    installed = false;
                    MessageBox.Show(error, Translate("Error"));
                }
            }
            RefreshGameControllers();

            /*var keyboardGameController = new GameController();
            var controllerView = new ControllerView(new ControllerViewModel(new ControllerModel(), keyboardGameController, log));
            controllerView.ViewModel.Model.CanStart = installed;
            Model.Controllers.Add(controllerView);*/

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
            }
            catch (Exception ex)
            {
                logger.Warning("Saving settings was unsuccessful.");
                string error = string.Format(Translate("SaveSettingsError"), SettingsFilePath) + Environment.NewLine + ex.Message;
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
                    break;
                case UpdateChecker.VersionCompare.NeedsUpgrade:
                    logger.Info("New version is available");
                    break;
                case UpdateChecker.VersionCompare.NewRelease:
                    break;
                case UpdateChecker.VersionCompare.UpToDate:
                    logger.Info("Version is up-to-date");
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        public void RefreshGameControllers()
        {
            directInputDevices.RefreshInputDevices();
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

        public void OpenDiagnostics()
        {
            IList<IDiagnostics> elements = directInputDevices.ConnectedDevices.Select(d => new InputDiagnostics(d)).OfType<IDiagnostics>().ToList();
            elements.Add(new InputDiagnostics(Keyboard.Instance));
            elements.Insert(0, new XInputDiagnostics());

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
            /*Model.Controllers.Remove(controllerView);
            logger.Info($"{controller.ToString()} is disconnected.");*/
        }

        private void HandleArgs()
        {
            foreach (var viewModel in Model.Controllers.Select(v => v.ViewModel))
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
