using System.Windows.Controls;
using XOutput.App.Devices.Input.RawInput;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI.View.InputView
{
    public partial class RawInputDeviceView : Grid, IViewBase<RawInputDeviceViewModel, RawInputDeviceModel>
    {
        public RawInputDeviceViewModel ViewModel => viewModel;

        private readonly RawInputDeviceViewModel viewModel;

        protected bool disposed = false;

        public RawInputDeviceView(RawInputDevice inputDevice)
        {
            viewModel = ApplicationContext.Global.Resolve<RawInputDeviceViewModel>();
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
