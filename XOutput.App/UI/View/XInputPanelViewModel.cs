using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
