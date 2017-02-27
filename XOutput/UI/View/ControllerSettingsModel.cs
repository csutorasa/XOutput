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
        private readonly ObservableCollection<AxisView> inputAxisViews = new ObservableCollection<AxisView>();
        public ObservableCollection<AxisView> InputAxisViews { get { return inputAxisViews; } }
        private readonly ObservableCollection<ButtonView> inputButtonViews = new ObservableCollection<ButtonView>();
        public ObservableCollection<ButtonView> InputButtonViews { get { return inputButtonViews; } }

        private readonly ObservableCollection<MappingView> mapperAxisViews = new ObservableCollection<MappingView>();
        public ObservableCollection<MappingView> MapperAxisViews { get { return mapperAxisViews; } }
        private readonly ObservableCollection<MappingView> mapperButtonViews = new ObservableCollection<MappingView>();
        public ObservableCollection<MappingView> MapperButtonViews { get { return mapperButtonViews; } }

        private readonly ObservableCollection<AxisView> xInputAxisViews = new ObservableCollection<AxisView>();
        public ObservableCollection<AxisView> XInputAxisViews { get { return xInputAxisViews; } }
        private readonly ObservableCollection<ButtonView> xInputButtonViews = new ObservableCollection<ButtonView>();
        public ObservableCollection<ButtonView> XInputButtonViews { get { return xInputButtonViews; } }

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
