using XOutput.DependencyInjection;

namespace XOutput.App.UI.View
{
    public class DirectInputPanelViewModel : ViewModelBase<DirectInputPanelModel>
    {
        [ResolverMethod]
        public DirectInputPanelViewModel(DirectInputPanelModel model) : base(model)
        {
            
        }
    }
}
