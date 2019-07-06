using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.UI.Component;

namespace XOutput.UI.Windows
{
    public class InputSettingsModel : ModelBase
    {
        private readonly ObservableCollection<IUpdatableView> inputAxisViews = new ObservableCollection<IUpdatableView>();
        public ObservableCollection<IUpdatableView> InputAxisViews => inputAxisViews;
        private readonly ObservableCollection<IUpdatableView> inputDPadViews = new ObservableCollection<IUpdatableView>();
        public ObservableCollection<IUpdatableView> InputDPadViews => inputDPadViews;
        private readonly ObservableCollection<IUpdatableView> inputButtonViews = new ObservableCollection<IUpdatableView>();
        public ObservableCollection<IUpdatableView> InputButtonViews => inputButtonViews;


        private string title;
        public string Title
        {
            get => title;
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        private string forceFeedbackText;
        public string ForceFeedbackText
        {
            get => forceFeedbackText;
            set
            {
                if (forceFeedbackText != value)
                {
                    forceFeedbackText = value;
                    OnPropertyChanged(nameof(ForceFeedbackText));
                }
            }
        }

        private string testButtonText;
        public string TestButtonText
        {
            get => testButtonText;
            set
            {
                if (testButtonText != value)
                {
                    testButtonText = value;
                    OnPropertyChanged(nameof(TestButtonText));
                }
            }
        }

        private bool forceFeedbackEnabled;
        public bool ForceFeedbackEnabled
        {
            get => forceFeedbackEnabled;
            set
            {
                if (forceFeedbackEnabled != value)
                {
                    forceFeedbackEnabled = value;
                    OnPropertyChanged(nameof(ForceFeedbackEnabled));
                }
            }
        }

        private bool forceFeedbackAvailable;
        public bool ForceFeedbackAvailable
        {
            get => forceFeedbackAvailable;
            set
            {
                if (forceFeedbackAvailable != value)
                {
                    forceFeedbackAvailable = value;
                    OnPropertyChanged(nameof(ForceFeedbackAvailable));
                }
            }
        }

        private bool isAdmin;
        public bool IsAdmin
        {
            get => isAdmin;
            set
            {
                if (isAdmin != value)
                {
                    isAdmin = value;
                    OnPropertyChanged(nameof(IsAdmin));
                }
            }
        }

        private bool hidGuardianAdded;
        public bool HidGuardianAdded
        {
            get => hidGuardianAdded;
            set
            {
                if (hidGuardianAdded != value)
                {
                    hidGuardianAdded = value;
                    OnPropertyChanged(nameof(HidGuardianAdded));
                }
            }
        }
    }
}
