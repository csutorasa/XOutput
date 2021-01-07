using System.Windows.Controls;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI.View
{
    public partial class GeneralPanel : Grid, IViewBase<GeneralPanelViewModel, GeneralPanelModel>
    {
        public GeneralPanelViewModel ViewModel => viewModel;

        private readonly GeneralPanelViewModel viewModel;

        [ResolverMethod]
        public GeneralPanel(GeneralPanelViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private async void Connect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await viewModel.Connect();
        }
    }
}
