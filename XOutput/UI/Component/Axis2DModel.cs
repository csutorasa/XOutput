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

        private int valuex;
        public int ValueX
        {
            get { return valuex; }
            set
            {
                if (valuex != value)
                {
                    valuex = value;
                    OnPropertyChanged(nameof(ValueX));
                }
            }
        }

        private int valuey;
        public int ValueY
        {
            get { return valuey; }
            set
            {
                if (valuey != value)
                {
                    valuey = value;
                    OnPropertyChanged(nameof(ValueY));
                }
            }
        }

        private int maxx;
        public int MaxX
        {
            get { return maxx; }
            set
            {
                if (maxx != value)
                {
                    maxx = value;
                    OnPropertyChanged(nameof(MaxX));
                }
            }
        }

        private int maxy;
        public int MaxY
        {
            get { return maxy; }
            set
            {
                if (maxy != value)
                {
                    maxy = value;
                    OnPropertyChanged(nameof(MaxY));
                }
            }
        }

        private bool twoD;
        public bool TwoD
        {
            get { return twoD; }
            set
            {
                if (twoD != value)
                {
                    twoD = value;
                    OnPropertyChanged(nameof(TwoD));
                }
            }
        }
    }
}
