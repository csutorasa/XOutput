using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices;

namespace XOutput.UI.Component
{
    public class Axis2DModel : ModelBase
    {
        public InputSource TypeX { get; set; }
        public InputSource TypeY { get; set; }

        private int valuex;
        public int ValueX
        {
            get => valuex;
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
            get => valuey;
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
            get => maxx;
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
            get => maxy;
            set
            {
                if (maxy != value)
                {
                    maxy = value;
                    OnPropertyChanged(nameof(MaxY));
                }
            }
        }
    }
}
