using System.Windows.Controls;
using XOutput.DependencyInjection;

namespace XOutput.App.UI.View
{
    public partial class DirectInputPanel : Grid, IViewBase<DirectInputPanelViewModel, DirectInputPanelModel>
    {
        public DirectInputPanelViewModel ViewModel => viewModel;

        private readonly DirectInputPanelViewModel viewModel;

        [ResolverMethod]
        public DirectInputPanel(DirectInputPanelViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
