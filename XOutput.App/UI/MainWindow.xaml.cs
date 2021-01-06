using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using XOutput.App.Configuration;
using XOutput.App.Devices.Input;
using XOutput.App.UI.View;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;
using XOutput.Core.External;
using XOutput.Core.Threading;

namespace XOutput.App.UI
{
    public partial class MainWindow : Window, IViewBase<MainWindowViewModel, MainWindowModel>
    {
        public MainWindowViewModel ViewModel => viewModel;

        private readonly MainWindowViewModel viewModel;
        private readonly CommandRunner commandRunner;
        private readonly ConfigurationManager configurationManager;

        private AppConfig appConfig;

        [ResolverMethod]
        public MainWindow(MainWindowViewModel viewModel, CommandRunner commandRunner, ConfigurationManager configurationManager, TranslationService translationService)
        {
            this.viewModel = viewModel;
            this.commandRunner = commandRunner;
            this.configurationManager = configurationManager;
            DataContext = viewModel;
            InitializeComponent();
            var helper = new WindowInteropHelper(this);
            WindowHandleStore.Handle = helper.EnsureHandle();
            appConfig = configurationManager.Load(() => new AppConfig(translationService.DefaultLanguage));
            if (!translationService.Load(appConfig.Language))
            {
                translationService.Load(translationService.DefaultLanguage);
            }
            ViewModel.Model.MainContent = ApplicationContext.Global.Resolve<GeneralPanel>();
            if (!appConfig.Minimized)
            {
                Show();
            }
        }

        private async void OpenClick(object sender, RoutedEventArgs e)
        {
            string address;
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    await socket.ConnectAsync("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    address = endPoint.Address.ToString();
                }
            } 
            catch
            {
                var ipAddresses = NetworkInterface.GetAllNetworkInterfaces()
                   .Where(i => i.NetworkInterfaceType == NetworkInterfaceType.Ethernet || i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                   .Where(i => i.OperationalStatus == OperationalStatus.Up)
                   .SelectMany(i => i.GetIPProperties().UnicastAddresses)
                   .Select(c => c.Address)
                   .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                   .Select(a => a.ToString())
                   .ToList();
                address = ipAddresses.FirstOrDefault() ?? "localhost";
            }
            int port = 8000;
            var process = commandRunner.CreatePowershell($"Start \"http://{address}:{port}\"");
            commandRunner.RunProcess(process);
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = (sender as TreeView).SelectedItem as TreeViewItem;
            if (item == GeneralTreeViewItem)
            {
                ViewModel.Model.MainContent = ApplicationContext.Global.Resolve<GeneralPanel>();
            }
            else if (item == WindowsApiKeyboardTreeViewItem)
            {
                ViewModel.Model.MainContent = ApplicationContext.Global.Resolve<WindowsApiKeyboardPanel>();
            }
            else if (item == WindowsApiMouseTreeViewItem)
            {
                ViewModel.Model.MainContent = ApplicationContext.Global.Resolve<WindowsApiMousePanel>();
            }
            else if (item == DirectInputTreeViewItem)
            {
                ViewModel.Model.MainContent = ApplicationContext.Global.Resolve<DirectInputPanel>();
            }
            else if (item == RawInputTreeViewItem)
            {
                ViewModel.Model.MainContent = ApplicationContext.Global.Resolve<RawInputPanel>();
            }
            else if (item == XInputTreeViewItem)
            {
                ViewModel.Model.MainContent = ApplicationContext.Global.Resolve<XInputPanel>();
            }
            else
            {
                throw new ArgumentException(nameof(sender));
            }
        }
    }
}
