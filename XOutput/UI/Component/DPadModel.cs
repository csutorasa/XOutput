using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.UI.Component
{
    public class DPadModel : ModelBase
    {
        private DPadDirection direction;
        public DPadDirection Direction
        {
            get { return direction; }
            set
            {
                if (direction != value)
                {
                    direction = value;
                    OnPropertyChanged(nameof(Direction));
                }
            }
        }

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

        private string label;
        public string Label
        {
            get { return label; }
            set
            {
                if (label != value)
                {
                    label = value;
                    OnPropertyChanged(nameof(Label));
                }
            }
        }
    }
}
