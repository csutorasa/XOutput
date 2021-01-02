using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI
{
    public class MainWindowViewModel : ViewModelBase<MainWindowModel>
    {
        [ResolverMethod]
        public MainWindowViewModel(MainWindowModel model) : base(model)
        {

        }
    }
}
