using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using XOutput.App.Devices;
using XOutput.App.UI;
using XOutput.Core;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Notifications;
using XOutput.Core.Resources;
using XOutput.Core.Versioning;

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
            this.appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SetLoggerConfiguration();

            Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Error);
            // AppDomain.CurrentDomain.FirstChanceException += (object sender, FirstChanceExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Info);
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => UnhandledException(e.ExceptionObject as Exception, LogLevel.Error);
            TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Error);

            logger.Info($"Starting XOutput app version: {appVersion}");

            var globalContext = ApplicationContext.Global;
            globalContext.AddFromConfiguration(typeof(CoreConfiguration));
            globalContext.AddFromConfiguration(typeof(AppConfiguration));
            globalContext.Discover(GetOrLoadAssemblies("XOutput.Core", "XOutput.Api", "XOutput.App"));
            logger.Info("Configuration classes are loaded");

            var mainWindow = ApplicationContext.Global.Resolve<MainWindow>();
            MainWindow = mainWindow;

            var hidGuardianManager = globalContext.Resolve<HidGuardianManager>();
            var notificationService = globalContext.Resolve<NotificationService>();

            if (hidGuardianManager.Installed)
            {
                try
                {
                    hidGuardianManager.ClearPid(Environment.ProcessId);
                    hidGuardianManager.SetPid(Environment.ProcessId);
                }
                catch(Exception)
                {
                    notificationService.Add(Notifications.HidGuardianRegistry, null, NotificationTypes.Warning);
                }
            }
            CheckUpdate(globalContext.Resolve<UpdateChecker>(), notificationService);
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

        private Task CheckUpdate(UpdateChecker updateChecker, NotificationService notificationService)
        {
            return updateChecker.CompareRelease(appVersion).ContinueWith(t => {
                switch (t.Result.Result) {
                    case VersionCompareValues.NeedsUpgrade:
                        notificationService.Add(Notifications.NeedsVersionUpgrade, new List<string>() { t.Result.LatestVersion });
                        break;
                    case VersionCompareValues.Error:
                        notificationService.Add(Notifications.VersionCheckError, new List<string>() { }, NotificationTypes.Warning);
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
