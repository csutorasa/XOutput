using XOutput.DependencyInjection;

namespace XOutput.App.UI.View
{
    public class RawInputPanelViewModel : ViewModelBase<RawInputPanelModel>
    {
        [ResolverMethod]
        public RawInputPanelViewModel(RawInputPanelModel model) : base(model)
        {
            
        }
    }
}
