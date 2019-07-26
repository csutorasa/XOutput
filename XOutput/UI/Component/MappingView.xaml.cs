using System.Windows;
using System.Windows.Controls;

namespace XOutput.UI.Component
{
    /// <summary>
    /// Interaction logic for MappingView.xaml
    /// </summary>
    public partial class MappingView : UserControl, IViewBase<MappingViewModel, MappingModel>
    {
        protected readonly MappingViewModel viewModel;
        public MappingViewModel ViewModel => viewModel;

        public MappingView(MappingViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        public void Refresh()
        {
            viewModel.Refresh();
        }

        private void ConfigureClick(object sender, RoutedEventArgs e)
        {
            viewModel.Configure();
            Refresh();
        }

        private void InvertClick(object sender, RoutedEventArgs e)
        {
            viewModel.Invert();
        }
    }
}
