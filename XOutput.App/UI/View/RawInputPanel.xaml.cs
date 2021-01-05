using System.Windows.Controls;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI.View
{
    public partial class RawInputPanel : Grid, IViewBase<RawInputPanelViewModel, RawInputPanelModel>
    {
        public RawInputPanelViewModel ViewModel => viewModel;

        private readonly RawInputPanelViewModel viewModel;

        [ResolverMethod]
        public RawInputPanel(RawInputPanelViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
