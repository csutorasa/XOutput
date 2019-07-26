using System.Windows.Controls;
using XOutput.Devices;

namespace XOutput.UI.Component
{
    /// <summary>
    /// Interaction logic for AxisView.xaml
    /// </summary>
    public partial class Axis2DView : UserControl, IUpdatableView, IViewBase<Axis2DViewModel, Axis2DModel>
    {
        protected readonly Axis2DViewModel viewModel;
        public Axis2DViewModel ViewModel => viewModel;

        public Axis2DView(Axis2DViewModel viewModel)
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
