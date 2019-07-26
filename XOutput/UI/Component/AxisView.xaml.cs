using System.Windows.Controls;
using XOutput.Devices;

namespace XOutput.UI.Component
{
    /// <summary>
    /// Interaction logic for AxisView.xaml
    /// </summary>
    public partial class AxisView : UserControl, IUpdatableView, IViewBase<AxisViewModel, AxisModel>
    {
        protected readonly AxisViewModel viewModel;
        public AxisViewModel ViewModel => viewModel;

        public AxisView(AxisViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        public void UpdateValues(IDevice device)
        {
            viewModel.UpdateValues(device);
        }
    }
}
