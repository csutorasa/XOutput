using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.App.Devices.Input.Mouse;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI.View
{
    public class WindowsApiMousePanelViewModel : ViewModelBase<WindowsApiMousePanelModel>
    {
        private readonly MouseDeviceProvider mouseDeviceProvider;

        [ResolverMethod]
        public WindowsApiMousePanelViewModel(WindowsApiMousePanelModel model, MouseDeviceProvider mouseDeviceProvider) : base(model)
        {
            this.mouseDeviceProvider = mouseDeviceProvider;
            Model.Enabled = mouseDeviceProvider.Enabled;
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.Enabled))
            {
                mouseDeviceProvider.Enabled = Model.Enabled;
            }
        }
    }
}
