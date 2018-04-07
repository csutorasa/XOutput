using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace XOutput
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
                    mutex = null;
                }
            }
        }
    }
}
