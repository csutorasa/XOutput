using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using XOutput.Devices.Input;
using XOutput.UI.Component;

namespace XOutput.UI.Windows
{
    public class ControllerSettingsModel : ModelBase
    {
        private readonly ObservableCollection<MappingView> mapperAxisViews = new ObservableCollection<MappingView>();
        public ObservableCollection<MappingView> MapperAxisViews => mapperAxisViews;
        private readonly ObservableCollection<MappingView> mapperDPadViews = new ObservableCollection<MappingView>();
        public ObservableCollection<MappingView> MapperDPadViews => mapperDPadViews;
        private readonly ObservableCollection<MappingView> mapperButtonViews = new ObservableCollection<MappingView>();
        public ObservableCollection<MappingView> MapperButtonViews => mapperButtonViews;

        private readonly ObservableCollection<IUpdatableView> xInputAxisViews = new ObservableCollection<IUpdatableView>();
        public ObservableCollection<IUpdatableView> XInputAxisViews => xInputAxisViews;
        private readonly ObservableCollection<IUpdatableView> xInputDPadViews = new ObservableCollection<IUpdatableView>();
        public ObservableCollection<IUpdatableView> XInputDPadViews => xInputDPadViews;
        private readonly ObservableCollection<IUpdatableView> xInputButtonViews = new ObservableCollection<IUpdatableView>();
        public ObservableCollection<IUpdatableView> XInputButtonViews => xInputButtonViews;

        private readonly ObservableCollection<ComboBoxItem> forceFeedbacks = new ObservableCollection<ComboBoxItem>();
        public ObservableCollection<ComboBoxItem> ForceFeedbacks => forceFeedbacks;

        private ComboBoxItem forceFeedback;
        public ComboBoxItem ForceFeedback
        {
            get => forceFeedback;
            set
            {
                if (forceFeedback != value)
                {
                    forceFeedback = value;
                    OnPropertyChanged(nameof(ForceFeedback));
                }
            }
        }

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

        private bool startWhenConnected;
        public bool StartWhenConnected
        {
            get => startWhenConnected;
            set
            {
                if (startWhenConnected != value)
                {
                    startWhenConnected = value;
                    OnPropertyChanged(nameof(StartWhenConnected));
                }
            }
        }
    }
}
