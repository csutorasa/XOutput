using System.Collections.ObjectModel;
using System.Windows;
using XOutput.App.Devices.Input.DirectInput;
using XOutput.App.Devices.Input.RawInput;
using XOutput.App.UI.View.InputView;
using XOutput.DependencyInjection;

namespace XOutput.App.UI
{
    public class MainWindowModel : ModelBase
    {
        private FrameworkElement mainContent;
        public FrameworkElement MainContent
        {
            get => mainContent;
            set {
                if (value != mainContent)
                {
                    if (mainContent is DirectInputDeviceView)
                    {
                        (mainContent as DirectInputDeviceView).DisposeViewModel();
                    }
                    if (mainContent is RawInputDeviceView)
                    {
                        (mainContent as RawInputDeviceView).DisposeViewModel();
                    }
                    mainContent = value;
                    OnPropertyChanged(nameof(MainContent));
                }
            }
        }

        private readonly ObservableCollection<DirectInputDevice> directInputs = new ObservableCollection<DirectInputDevice>();
        public ObservableCollection<DirectInputDevice> DirectInputs => directInputs;


        private readonly ObservableCollection<RawInputDevice> rawInputs = new ObservableCollection<RawInputDevice>();
        public ObservableCollection<RawInputDevice> RawInputs => rawInputs;


        private readonly ObservableCollection<object> xInputs = new ObservableCollection<object>();
        public ObservableCollection<object> XInputs => xInputs;

        [ResolverMethod]
        public MainWindowModel()
        {

        }
    }
}
