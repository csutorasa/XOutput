using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI.View.InputView
{
    public class DirectInputDeviceViewModel : DisposableViewModelBase<DirectInputDeviceModel>
    {
        [ResolverMethod]
        public DirectInputDeviceViewModel(DirectInputDeviceModel model) : base(model)
        {
            
        }
    }
}
