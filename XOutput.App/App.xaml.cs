using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using XOutput.App.Devices;
using XOutput.App.Devices.Input;
using XOutput.App.UI;
using XOutput.Client;
using XOutput.DependencyInjection;
using XOutput.Notifications;
using XOutput.Resources;
using XOutput.Versioning;

namespace XOutput.App
{
    public partial class App : Application
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly string appVersion;

        public App()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string cwd = Path.GetDirectoryName(exePath);
            Directory.SetCurrentDirectory(cwd);
            appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SetLoggerConfiguration();

            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            // AppDomain.CurrentDomain.FirstChanceException += (object sender, FirstChanceExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Info);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            logger.Info($"Starting XOutput app version: {appVersion}");

            var globalContext = ApplicationContext.Global;
            globalContext.AddFromConfiguration(typeof(CoreConfiguration));
            globalContext.AddFromConfiguration(typeof(ClientConfiguration));
            globalContext.AddFromConfiguration(typeof(AppConfiguration));
            globalContext.Discover(GetOrLoadAssemblies("XOutput.Core", "XOutput.Api", "XOutput.Client", "XOutput.App"));
            logger.Info("Configuration classes are loaded");

            var mainWindow = ApplicationContext.Global.Resolve<MainWindow>();
            MainWindow = mainWindow;

            var hidGuardianManager = globalContext.Resolve<HidGuardianManager>();
            var notificationService = globalContext.Resolve<NotificationService>();
            globalContext.Resolve<InputDeviceManager>();

            if (hidGuardianManager.Installed)
            {
                try
                {
                    hidGuardianManager.ClearPid(Environment.ProcessId);
                    hidGuardianManager.SetPid(Environment.ProcessId);
                }
                catch(Exception)
                {
                    notificationService.Add(Notifications.Notifications.HidGuardianRegistry, null, NotificationTypes.Warning);
                }
            }
            CheckUpdate(globalContext.Resolve<UpdateChecker>(), notificationService);
        }

        [SecurityCritical]
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            UnhandledException(e.ExceptionObject as Exception, LogLevel.Error);
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            UnhandledException(e.Exception, LogLevel.Error);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            UnhandledException(e.Exception, LogLevel.Error);
        }

        private static void SetLoggerConfiguration()
        {
            try
            {
                if (File.Exists("nlog.config"))
                {
                    LogManager.Configuration = new XmlLoggingConfiguration(XmlReader.Create(File.OpenRead("nlog.config")));
                    return;
                }
            }
            catch
            {
                // Cannot create logger
            }
            LogManager.Configuration = new XmlLoggingConfiguration(XmlReader.Create(AssemblyResourceManager.GetResourceStream("XOutput.App.nlog.config")));
        }

        private void CheckUpdate(UpdateChecker updateChecker, NotificationService notificationService)
        {
            updateChecker.CompareReleaseAsync(appVersion).ContinueWith(t => {
                switch (t.Result.Result) {
                    case VersionCompareValues.NeedsUpgrade:
                        notificationService.Add(Notifications.Notifications.NeedsVersionUpgrade, new List<string>() { t.Result.LatestVersion });
                        break;
                    case VersionCompareValues.Error:
                        notificationService.Add(Notifications.Notifications.VersionCheckError, new List<string>() { }, NotificationTypes.Warning);
                        break;
                }
            });
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ApplicationContext.Global.Close();
        }

        public static void UnhandledException(Exception exceptionObject, LogLevel level)
        {
            logger.Log(level, exceptionObject);
        }

        private static IEnumerable<Assembly> GetOrLoadAssemblies(params string[] assemblyNames)
        {
            return assemblyNames.Select(assemblyName =>
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyName);
                if (assembly != null)
                {
                    return assembly;
                }
                return AppDomain.CurrentDomain.Load(assemblyName);
            }).ToList();
        }
    }
}
