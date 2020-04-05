using System.Windows;
using System.Windows.Interop;
using XOutput.Core.DependencyInjection;

namespace XOutput.App
{
    public partial class MainWindow : Window
    {
        [ResolverMethod]
        public MainWindow()
        {
            InitializeComponent();
            new WindowInteropHelper(this).EnsureHandle();
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
