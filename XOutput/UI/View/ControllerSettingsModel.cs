using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.UI.Component;

namespace XOutput.UI.View
{
    public class ControllerSettingsModel : ModelBase
    {
        private readonly ObservableCollection<IUpdatableView> inputAxisViews = new ObservableCollection<IUpdatableView>();
        public ObservableCollection<IUpdatableView> InputAxisViews => inputAxisViews;
        private readonly ObservableCollection<IUpdatableView> inputButtonViews = new ObservableCollection<IUpdatableView>();
        public ObservableCollection<IUpdatableView> InputButtonViews => inputButtonViews;

        private readonly ObservableCollection<MappingView> mapperAxisViews = new ObservableCollection<MappingView>();
        public ObservableCollection<MappingView> MapperAxisViews => mapperAxisViews;
        private readonly ObservableCollection<MappingView> mapperButtonViews = new ObservableCollection<MappingView>();
        public ObservableCollection<MappingView> MapperButtonViews => mapperButtonViews;

        private readonly ObservableCollection<IUpdatableView> xInputAxisViews = new ObservableCollection<IUpdatableView>();
        public ObservableCollection<IUpdatableView> XInputAxisViews => xInputAxisViews;
        private readonly ObservableCollection<IUpdatableView> xInputButtonViews = new ObservableCollection<IUpdatableView>();
        public ObservableCollection<IUpdatableView> XInputButtonViews => xInputButtonViews;

        private string _DPadText;
        public string DPadText
        {
            get { return _DPadText; }
            set
            {
                if (_DPadText != value)
                {
                    _DPadText = value;
                    OnPropertyChanged(nameof(DPadText));
                }
            }
        }
        private string _xDPadText;
        public string XDPadText
        {
            get { return _xDPadText; }
            set
            {
                if (_xDPadText != value)
                {
                    _xDPadText = value;
                    OnPropertyChanged(nameof(XDPadText));
                }
            }
        }
        private string _mapperDPadText;
        public string MapperDPadText
        {
            get { return _mapperDPadText; }
            set
            {
                if (_mapperDPadText != value)
                {
                    _mapperDPadText = value;
                    OnPropertyChanged(nameof(MapperDPadText));
                }
            }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }
    }
}
