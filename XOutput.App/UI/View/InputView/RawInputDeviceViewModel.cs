using XOutput.DependencyInjection;

namespace XOutput.App.UI.View.InputView
{
    public class RawInputDeviceViewModel : DisposableViewModelBase<RawInputDeviceModel>
    {
        [ResolverMethod]
        public RawInputDeviceViewModel(RawInputDeviceModel model) : base(model)
        {
            
        }
    }
}
