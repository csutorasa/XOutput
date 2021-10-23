using XOutput.DependencyInjection;

namespace XOutput.App.UI.View
{
    public class XInputPanelViewModel : ViewModelBase<XInputPanelModel>
    {
        [ResolverMethod]
        public XInputPanelViewModel(XInputPanelModel model) : base(model)
        {
            
        }
    }
}
