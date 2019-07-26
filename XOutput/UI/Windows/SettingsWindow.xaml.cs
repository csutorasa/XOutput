using System.Windows;

namespace XOutput.UI.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, IViewBase<SettingsViewModel, SettingsModel>
    {
        private readonly SettingsViewModel viewModel;
        public SettingsViewModel ViewModel => viewModel;

        public SettingsWindow(SettingsViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
