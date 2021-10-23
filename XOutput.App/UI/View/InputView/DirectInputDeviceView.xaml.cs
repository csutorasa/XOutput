using System.Windows.Controls;
using XOutput.App.Devices.Input.DirectInput;
using XOutput.DependencyInjection;

namespace XOutput.App.UI.View.InputView
{
    public partial class DirectInputDeviceView : Grid, IViewBase<DirectInputDeviceViewModel, DirectInputDeviceModel>
    {
        public DirectInputDeviceViewModel ViewModel => viewModel;

        private readonly DirectInputDeviceViewModel viewModel;

        protected bool disposed = false;

        public DirectInputDeviceView(DirectInputDevice inputDevice)
        {
            viewModel = ApplicationContext.Global.Resolve<DirectInputDeviceViewModel>();
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
