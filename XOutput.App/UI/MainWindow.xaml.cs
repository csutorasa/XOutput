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
using XOutput.App.Devices.Input.DirectInput.Native;
using XOutput.App.UI.View;
using XOutput.Configuration;
using XOutput.DependencyInjection;
using XOutput.External;
using XOutput.Rest;
using XOutput.Threading;

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
        public MainWindow(MainWindowViewModel viewModel, CommandRunner commandRunner, ConfigurationManager configurationManager,
            TranslationService translationService, DynamicHttpClientProvider dynamicHttpClientProvider, InputDeviceManager inputDeviceManager)
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
            if (appConfig.ServerUrl != null)
            {
                var uri = new Uri($"http://{appConfig.ServerUrl}/api/");
                dynamicHttpClientProvider.SetBaseAddress(uri);
                if (appConfig.AutoConnect)
                {
                    // TODO autoconnect
                }
            }
            if (!appConfig.Minimized)
            {
                Show();
            }
            IntPtr value;
            var hinst = DInput8.GetModuleHandle(null);
            var guid = IID.IID_IDirectInput8W;
            var x = DInput8.DirectInput8Create(hinst, 0x00000800, guid, out value, IntPtr.Zero);
            var err = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
            var directInput8W = new DirectInput8W(value);
            directInput8W.GetLifetimeService();
            inputDeviceManager.Start();
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
            var selectedItem = (sender as TreeView).SelectedItem;
            if (selectedItem is IInputDevice)
            {
                ViewModel.DeviceSelected(selectedItem as IInputDevice);
            }
            else if (selectedItem is TreeViewItem)
            {
                var treeItem = selectedItem as TreeViewItem;
                if (treeItem == GeneralTreeViewItem)
                {
                    ViewModel.Model.MainContent = ApplicationContext.Global.Resolve<GeneralPanel>();
                }
                else if (treeItem == WindowsApiKeyboardTreeViewItem)
                {
                    ViewModel.Model.MainContent = ApplicationContext.Global.Resolve<WindowsApiKeyboardPanel>();
                }
                else if (treeItem == WindowsApiMouseTreeViewItem)
                {
                    ViewModel.Model.MainContent = ApplicationContext.Global.Resolve<WindowsApiMousePanel>();
                }
                else if (treeItem == DirectInputTreeViewItem)
                {
                    ViewModel.Model.MainContent = ApplicationContext.Global.Resolve<DirectInputPanel>();
                }
                else if (treeItem == RawInputTreeViewItem)
                {
                    ViewModel.Model.MainContent = ApplicationContext.Global.Resolve<RawInputPanel>();
                }
                else if (treeItem == XInputTreeViewItem)
                {
                    ViewModel.Model.MainContent = ApplicationContext.Global.Resolve<XInputPanel>();
                }
            }
            else
            {
                throw new ArgumentException(nameof(sender));
            }
        }
    }
}
