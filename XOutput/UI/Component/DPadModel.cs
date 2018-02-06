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
    }
}
