using System.Windows.Controls;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI.View
{
    public partial class WindowsApiKeyboardPanel : Grid, IViewBase<WindowsApiKeyboardPanelViewModel, WindowsApiKeyboardPanelModel>
    {
        public WindowsApiKeyboardPanelViewModel ViewModel => viewModel;

        private readonly WindowsApiKeyboardPanelViewModel viewModel;

        [ResolverMethod]
        public WindowsApiKeyboardPanel(WindowsApiKeyboardPanelViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
