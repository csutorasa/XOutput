using System.Collections.ObjectModel;
using XOutput.App.Devices.Input.Keyboard;
using XOutput.DependencyInjection;

namespace XOutput.App.UI.View
{
    public class WindowsApiKeyboardPanelModel : ModelBase
    {
        private bool enabled;
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }
        }

        private readonly ObservableCollection<KeyboardButton> pressedButtons = new ObservableCollection<KeyboardButton>();
        public ObservableCollection<KeyboardButton> PressedButtons => pressedButtons;

        [ResolverMethod]
        public WindowsApiKeyboardPanelModel()
        {

        }
    }
}
