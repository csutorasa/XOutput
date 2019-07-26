using System.Windows.Controls;

namespace XOutput.UI.Component
{
    /// <summary>
    /// Interaction logic for MappingView.xaml
    /// </summary>
    public partial class DiagnosticsItemView : UserControl, IViewBase<DiagnosticsItemViewModel, DiagnosticsItemModel>
    {
        protected readonly DiagnosticsItemViewModel viewModel;
        public DiagnosticsItemViewModel ViewModel => viewModel;

        public DiagnosticsItemView(DiagnosticsItemViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
