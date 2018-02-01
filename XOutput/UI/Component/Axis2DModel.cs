using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.UI.Component
{
    public class Axis2DModel : ModelBase
    {
        public Enum TypeX { get; set; }
        public Enum TypeY { get; set; }

        private string _labelx;
        public string LabelX
        {
            get { return _labelx; }
            set
            {
                if (_labelx != value)
                {
                    _labelx = value;
                    OnPropertyChanged(nameof(LabelX));
                }
            }
        }

        private string _labely;
        public string LabelY
        {
            get { return _labely; }
            set
            {
                if (_labely != value)
                {
                    _labely = value;
                    OnPropertyChanged(nameof(LabelY));
                }
            }
        }

        private int _valuex;
        public int ValueX
        {
            get { return _valuex; }
            set
            {
                if (_valuex != value)
                {
                    _valuex = value;
                    OnPropertyChanged(nameof(ValueX));
                }
            }
        }

        private int _valuey;
        public int ValueY
        {
            get { return _valuey; }
            set
            {
                if (_valuey != value)
                {
                    _valuey = value;
                    OnPropertyChanged(nameof(ValueY));
                }
            }
        }

        private int _maxx;
        public int MaxX
        {
            get { return _maxx; }
            set
            {
                if (_maxx != value)
                {
                    _maxx = value;
                    OnPropertyChanged(nameof(MaxX));
                }
            }
        }

        private int _maxy;
        public int MaxY
        {
            get { return _maxy; }
            set
            {
                if (_maxy != value)
                {
                    _maxy = value;
                    OnPropertyChanged(nameof(MaxY));
                }
            }
        }
    }
}
