using System.Windows.Controls;
using XOutput.Devices;

namespace XOutput.UI.Component
{
    /// <summary>
    /// Interaction logic for AxisView.xaml
    /// </summary>
    public partial class DPadView : UserControl, IUpdatableView, IViewBase<DPadViewModel, DPadModel>
    {
        protected readonly DPadViewModel viewModel;
        public DPadViewModel ViewModel => viewModel;

        public DPadView(DPadViewModel viewModel)
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
