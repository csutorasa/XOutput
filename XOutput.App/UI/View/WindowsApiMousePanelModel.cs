using System.Collections.ObjectModel;
using System.Windows;
using XOutput.App.Devices.Input;
using XOutput.App.Devices.Input.Mouse;
using XOutput.DependencyInjection;

namespace XOutput.App.UI.View
{
    public class WindowsApiMousePanelModel : ModelBase
    {
        private bool enabled;
        public bool Enabled
        {
            get => enabled;
            set 
            {
                if (enabled != value) {
                    enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }
        }

        private Visibility leftButtonVisibility = Visibility.Hidden;
        public Visibility LeftButtonVisibility
        {
            get => leftButtonVisibility;
            set 
            {
                if (leftButtonVisibility != value) {
                    leftButtonVisibility = value;
                    OnPropertyChanged(nameof(LeftButtonVisibility));
                }
            }
        }

        private Visibility rightButtonVisibility = Visibility.Hidden;
        public Visibility RightButtonVisibility
        {
            get => rightButtonVisibility;
            set 
            {
                if (rightButtonVisibility != value) {
                    rightButtonVisibility = value;
                    OnPropertyChanged(nameof(RightButtonVisibility));
                }
            }
        }

        private Visibility middleButtonVisibility = Visibility.Hidden;
        public Visibility MiddleButtonVisibility
        {
            get => middleButtonVisibility;
            set 
            {
                if (middleButtonVisibility != value) {
                    middleButtonVisibility = value;
                    OnPropertyChanged(nameof(MiddleButtonVisibility));
                }
            }
        }

        private Visibility x1ButtonVisibility = Visibility.Hidden;
        public Visibility X1ButtonVisibility
        {
            get => x1ButtonVisibility;
            set 
            {
                if (x1ButtonVisibility != value) {
                    x1ButtonVisibility = value;
                    OnPropertyChanged(nameof(X1ButtonVisibility));
                }
            }
        }

        private Visibility x2ButtonVisibility = Visibility.Hidden;
        public Visibility X2ButtonVisibility
        {
            get => x2ButtonVisibility;
            set 
            {
                if (x2ButtonVisibility != value) {
                    x2ButtonVisibility = value;
                    OnPropertyChanged(nameof(X2ButtonVisibility));
                }
            }
        }

        [ResolverMethod]
        public WindowsApiMousePanelModel()
        {

        }
    }
}
