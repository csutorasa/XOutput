using System.Windows.Controls;
using XOutput.DependencyInjection;

namespace XOutput.App.UI.View
{
    public partial class XInputPanel : Grid, IViewBase<XInputPanelViewModel, XInputPanelModel>
    {
        public XInputPanelViewModel ViewModel => viewModel;

        private readonly XInputPanelViewModel viewModel;

        [ResolverMethod]
        public XInputPanel(XInputPanelViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
