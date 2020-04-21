using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Interop;
using XOutput.Core.DependencyInjection;
using XOutput.Core.External;
using XOutput.Core.Threading;

namespace XOutput.App
{
    public partial class MainWindow : Window
    {
        private readonly CommandRunner commandRunner;

        [ResolverMethod]
        public MainWindow(CommandRunner commandRunner)
        {
            this.commandRunner = commandRunner;
            InitializeComponent();
            var helper = new WindowInteropHelper(this);
            WindowHandleStore.Handle = helper.EnsureHandle();
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            var ipAddresses = NetworkInterface.GetAllNetworkInterfaces()
               .Where(i => i.NetworkInterfaceType == NetworkInterfaceType.Ethernet || i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
               .Where(i => i.OperationalStatus == OperationalStatus.Up)
               .SelectMany(i => i.GetIPProperties().UnicastAddresses)
               .Select(c => c.Address)
               .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
               .Select(a => a.ToString())
               .ToList();
            var address = ipAddresses.FirstOrDefault() ?? "localhost";
            int port = 8000;
            var process = commandRunner.CreatePowershell($"Start \"http://{address}:{port}\"");
            commandRunner.RunProcess(process);
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
