using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.UI.Component;

namespace XOutput.UI.View
{
    public class ControllerSettingsViewModel : ViewModelBase
    {
        private readonly ObservableCollection<AxisView> directInputAxisViews = new ObservableCollection<AxisView>();
        public ObservableCollection<AxisView> DirectInputAxisViews { get { return directInputAxisViews; } }
        private readonly ObservableCollection<ButtonView> directInputButtonViews = new ObservableCollection<ButtonView>();
        public ObservableCollection<ButtonView> DirectInputButtonViews { get { return directInputButtonViews; } }

        private readonly ObservableCollection<MappingView> mapperAxisViews = new ObservableCollection<MappingView>();
        public ObservableCollection<MappingView> MapperAxisViews { get { return mapperAxisViews; } }
        private readonly ObservableCollection<MappingView> mapperButtonViews = new ObservableCollection<MappingView>();
        public ObservableCollection<MappingView> MapperButtonViews { get { return mapperButtonViews; } }

        private readonly ObservableCollection<AxisView> xInputAxisViews = new ObservableCollection<AxisView>();
        public ObservableCollection<AxisView> XInputAxisViews { get { return xInputAxisViews; } }
        private readonly ObservableCollection<ButtonView> xInputButtonViews = new ObservableCollection<ButtonView>();
        public ObservableCollection<ButtonView> XInputButtonViews { get { return xInputButtonViews; } }

        private string _directDPadText;
        public string DirectDPadText
        {
            get { return _directDPadText; }
            set
            {
                if (_directDPadText != value)
                {
                    _directDPadText = value;
                    OnPropertyChanged(nameof(DirectDPadText));
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
