using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices;

namespace XOutput.UI.Component
{
    public class AxisModel : ModelBase
    {
        private InputSource type;
        public InputSource Type
        {
            get => type;
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }
        private int value;
        public int Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged(nameof(AxisModel.Value));
                }
            }
        }
        private int max;
        public int Max
        {
            get => max;
            set
            {
                if (max != value)
                {
                    max = value;
                    OnPropertyChanged(nameof(Max));
                }
            }
        }
    }
}
