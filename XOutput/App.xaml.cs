using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using XOutput.Tools;
using XOutput.UI.Windows;

namespace XOutput
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static MainWindowViewModel mainWindowViewModel;

        public App()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string cwd = Path.GetDirectoryName(exePath);
            Directory.SetCurrentDirectory(cwd);
        }

        [STAThread]
        public static void Main()
        {
            Mutex mutex = new Mutex(false, "XOutputRunningAlreadyMutex");
            try
            {
                if (mutex.WaitOne(0, false))
                {
                    App app = new App();
                    app.InitializeComponent();
                    app.Run();
                    mainWindowViewModel.Dispose();
                }
                else
                {
                    MessageBox.Show("An instance of the application is already running.");
                }
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.Close();
                }
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            mainWindowViewModel = new MainWindowViewModel(new MainWindowModel(), Dispatcher);
            var mainWindow = new MainWindow(mainWindowViewModel);
            MainWindow = mainWindow;
            if (!ArgumentParser.Instance.Minimized)
            {
                mainWindow.Show();
            }
        }
    }
}
