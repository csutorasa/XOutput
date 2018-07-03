using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.UI.Component;

namespace XOutput.UI.Windows
{
    public class InputDeviceSettingsModel : ModelBase
    {
        private readonly ObservableCollection<IUpdatableView> inputAxisViews = new ObservableCollection<IUpdatableView>();
        public ObservableCollection<IUpdatableView> InputAxisViews => inputAxisViews;
        private readonly ObservableCollection<IUpdatableView> inputDPadViews = new ObservableCollection<IUpdatableView>();
        public ObservableCollection<IUpdatableView> InputDPadViews => inputDPadViews;
        private readonly ObservableCollection<IUpdatableView> inputButtonViews = new ObservableCollection<IUpdatableView>();
        public ObservableCollection<IUpdatableView> InputButtonViews => inputButtonViews;

        private readonly ObservableCollection<int> dpads = new ObservableCollection<int>();
        public ObservableCollection<int> Dpads => dpads;

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
    }
}
