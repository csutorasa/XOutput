using System.Windows.Controls;
using XOutput.DependencyInjection;

namespace XOutput.App.UI.View
{
    public partial class WindowsApiMousePanel : Grid, IViewBase<WindowsApiMousePanelViewModel, WindowsApiMousePanelModel>
    {
        public WindowsApiMousePanelViewModel ViewModel => viewModel;

        private readonly WindowsApiMousePanelViewModel viewModel;

        [ResolverMethod]
        public WindowsApiMousePanel(WindowsApiMousePanelViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
