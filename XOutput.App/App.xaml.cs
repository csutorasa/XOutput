using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using XOutput.Core;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Notifications;
using XOutput.Core.Resources;
using XOutput.Server.Emulation.HidGuardian;

namespace XOutput.App
{
    public partial class App : Application
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private Server.Server server;

        public App()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string cwd = Path.GetDirectoryName(exePath);
            Directory.SetCurrentDirectory(cwd);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SetLoggerConfiguration();

            Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Error);
            // AppDomain.CurrentDomain.FirstChanceException += (object sender, FirstChanceExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Info);
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => UnhandledException(e.ExceptionObject as Exception, LogLevel.Error);
            TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Error);

            var globalContext = ApplicationContext.Global;
            globalContext.AddFromConfiguration(typeof(CoreConfiguration));
            globalContext.AddFromConfiguration(typeof(ApiConfiguration));
            globalContext.AddFromConfiguration(typeof(AppConfiguration));
            globalContext.Discover(GetOrLoadAssemblies("XOutput.Core", "XOutput.Api", "XOutput.Devices", "XOutput.Emulation", "XOutput.Server"));
            var configurationManager = globalContext.Resolve<ConfigurationManager>();

            var mainWindow = ApplicationContext.Global.Resolve<MainWindow>();
            MainWindow = mainWindow;

            var hidGuardianManager = globalContext.Resolve<HidGuardianManager>();
            hidGuardianManager.ClearPid(Process.GetCurrentProcess().Id);

            string settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "config", "server.json");
            if (true)
            {
                try
                {
                    hidGuardianManager.SetPid(Process.GetCurrentProcess().Id);
                }
                catch(Exception)
                {
                    globalContext.Resolve<NotificationService>().Add(Notifications.HidGuardianRegistry, null, NotificationTypes.Warning);
                }
            }
            server = globalContext.Resolve<Server.Server>();
            server.GetHost().StartAsync();
        }

        private void SetLoggerConfiguration()
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

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ApplicationContext.Global.Close();
        }

        public static void UnhandledException(Exception exceptionObject, LogLevel level)
        {
            logger.Log(level, exceptionObject);
        }

        private IEnumerable<Assembly> GetOrLoadAssemblies(params string[] assemblyNames)
        {
            return assemblyNames.Select(assemblyName =>
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName == assemblyName).FirstOrDefault();
                if (assembly != null)
                {
                    return assembly;
                }
                return AppDomain.CurrentDomain.Load(assemblyName);
            }).ToList();
        }
    }
}
