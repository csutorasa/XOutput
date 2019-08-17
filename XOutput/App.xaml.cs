using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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
        private readonly Mutex mutex = new Mutex(false, "XOutputRunningAlreadyMutex");

        public App()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string cwd = Path.GetDirectoryName(exePath);
            Directory.SetCurrentDirectory(cwd);
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
            if (mutex.WaitOne(0, false))
            {
                try {
                    mainWindowViewModel = new MainWindowViewModel(new MainWindowModel(), Dispatcher);
                    var mainWindow = new MainWindow(mainWindowViewModel);
                    MainWindow = mainWindow;
                    if (!ArgumentParser.Instance.Minimized)
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
                MessageBox.Show("An instance of the application is already running.");
                Application.Current.Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            mainWindowViewModel.Dispose();
            mutex?.Close();
        }
    }
}
