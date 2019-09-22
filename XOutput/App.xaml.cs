using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using XOutput.Logging;
using XOutput.Tools;
using XOutput.UI.Windows;

namespace XOutput
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(App));

        private MainWindowViewModel mainWindowViewModel;
        private SingleInstanceProvider singleInstanceProvider;
        private ArgumentParser argumentParser;

        public App()
        {
            DependencyEmbedder dependencyEmbedder = new DependencyEmbedder();
            dependencyEmbedder.AddPackage("Newtonsoft.Json");
            dependencyEmbedder.AddPackage("SharpDX.DirectInput");
            dependencyEmbedder.AddPackage("SharpDX");
            dependencyEmbedder.AddPackage("Hardcodet.Wpf.TaskbarNotification");
            dependencyEmbedder.AddPackage("Nefarius.ViGEm.Client");
            dependencyEmbedder.Initialize();
            string exePath = Assembly.GetExecutingAssembly().Location;
            string cwd = Path.GetDirectoryName(exePath);
            Directory.SetCurrentDirectory(cwd);
            singleInstanceProvider = new SingleInstanceProvider();
            argumentParser = new ArgumentParser();

            ApplicationContext globalContext = ApplicationContext.Global;
            globalContext.Resolvers.Add(Resolver.CreateSingleton(argumentParser));
#if !DEBUG
            Dispatcher.UnhandledException += async (object sender, DispatcherUnhandledExceptionEventArgs e) => await UnhandledException(e.Exception);
#endif
        }

        public async Task UnhandledException(Exception exceptionObject)
        {
            await logger.Error(exceptionObject);
            MessageBox.Show(exceptionObject.Message + Environment.NewLine + exceptionObject.StackTrace);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (singleInstanceProvider.TryGetLock())
            {
                singleInstanceProvider.StartNamedPipe();
                try {
                    mainWindowViewModel = new MainWindowViewModel(new MainWindowModel(), Dispatcher);
                    var mainWindow = new MainWindow(mainWindowViewModel, argumentParser);
                    MainWindow = mainWindow;
                    singleInstanceProvider.ShowEvent += mainWindow.ForceShow;
                    if (!argumentParser.Minimized)
                    {
                        mainWindow.Show();
                    }
                } catch (Exception ex) {
                    logger.Error(ex);
                    MessageBox.Show(ex.ToString());
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
            singleInstanceProvider.StopNamedPipe();
            singleInstanceProvider.Close();
        }
    }
}
