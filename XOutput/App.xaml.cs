using NLog;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using XOutput.Core;
using XOutput.Core.DependencyInjection;
using XOutput.Tools;
using XOutput.UI.Windows;

namespace XOutput
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private MainWindowViewModel mainWindowViewModel;

        public App()
        {
            Dispatcher.UnhandledException += (object sender, DispatcherUnhandledExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Error);
            AppDomain.CurrentDomain.FirstChanceException += (object sender, FirstChanceExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Info);
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => UnhandledException(e.ExceptionObject as Exception, LogLevel.Error);
            TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs e) => UnhandledException(e.Exception, LogLevel.Error);
            string exePath = Assembly.GetExecutingAssembly().Location;
            string cwd = Path.GetDirectoryName(exePath);
            Directory.SetCurrentDirectory(cwd);
        }

        public void UnhandledException(Exception exceptionObject, LogLevel level)
        {
            logger.Log(level, exceptionObject);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ApplicationContext globalContext = ApplicationContext.Global;
            globalContext.Resolvers.Add(Resolver.CreateSingleton(Dispatcher));
            globalContext.AddFromConfiguration(typeof(CoreConfiguration));
            globalContext.AddFromConfiguration(typeof(ApiConfiguration));
            globalContext.AddFromConfiguration(typeof(ApplicationConfiguration));
            globalContext.AddFromConfiguration(typeof(UI.UIConfiguration));

            var singleInstanceProvider = globalContext.Resolve<SingleInstanceProvider>();
            var argumentParser = globalContext.Resolve<ArgumentParser>();
            globalContext.Discover();
            if (singleInstanceProvider.TryGetLock())
            {
                singleInstanceProvider.StartNamedPipe();
                try
                {
                    var mainWindow = ApplicationContext.Global.Resolve<MainWindow>();
                    mainWindowViewModel = mainWindow.ViewModel;
                    MainWindow = mainWindow;
                    singleInstanceProvider.ShowEvent += mainWindow.ForceShow;
                    if (!argumentParser.Minimized)
                    {
                        mainWindow.Show();
                    }
#if !DEBUG
                    ApplicationContext.Global.Resolve<Devices.Input.Mouse.MouseHook>().StartHook();              
#endif
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    Application.Current.Shutdown();
                }
            }
            else
            {
                singleInstanceProvider.Notify();
                Application.Current.Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            mainWindowViewModel?.Dispose();
            ApplicationContext.Global.Close();
        }
    }
}
