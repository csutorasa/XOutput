using System.Windows.Controls;
using XOutput.Devices;

namespace XOutput.UI.Component
{
    /// <summary>
    /// Interaction logic for ButtonView.xaml
    /// </summary>
    public partial class ButtonView : UserControl, IUpdatableView, IViewBase<ButtonViewModel, ButtonModel>
    {
        protected readonly ButtonViewModel viewModel;
        public ButtonViewModel ViewModel => viewModel;

        public ButtonView(ButtonViewModel viewModel)
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
