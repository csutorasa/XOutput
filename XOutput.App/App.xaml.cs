using NLog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using XOutput.Core;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

namespace XOutput.App
{
    public partial class App : Application
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private IHost server;
        private static string settingsPath;

        public App() {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string cwd = Path.GetDirectoryName(exePath);
            Directory.SetCurrentDirectory(cwd);
            settingsPath = Path.Combine(cwd, "config", "server.json");
            
            Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Error);
            AppDomain.CurrentDomain.FirstChanceException += (object sender, FirstChanceExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Info);
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => UnhandledException(e.ExceptionObject as Exception, LogLevel.Error);
            TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Error);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var globalContext = ApplicationContext.Global;
            globalContext.AddFromConfiguration(typeof(CoreConfiguration));
            globalContext.AddFromConfiguration(typeof(ApiConfiguration));
            globalContext.AddFromConfiguration(typeof(AppConfiguration));
            globalContext.Discover(GetOrLoadAssemblies("XOutput.Core", "XOutput.Core", "XOutput.Devices", "XOutput.Server"));
            var configurationManager = globalContext.Resolve<ConfigurationManager>();
            
            var mainWindow = ApplicationContext.Global.Resolve<MainWindow>();
            MainWindow = mainWindow;

            server = globalContext.Resolve<IHost>();
            server.StartAsync();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ApplicationContext.Global.Close();
        }

        public void UnhandledException(Exception exceptionObject, LogLevel level)
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
