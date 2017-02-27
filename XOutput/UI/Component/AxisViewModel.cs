using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.UI.Component
{
    public class AxisViewModel : ModelBase
    {
        public Enum Type { get; set; }

        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != value)
                {
                    _label = value;
                    OnPropertyChanged(nameof(Label));
                }
            }
        }
        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }
        private int _max;
        public int Max
        {
            get { return _max; }
            set
            {
                if (_max != value)
                {
                    _max = value;
                    OnPropertyChanged(nameof(Max));
                }
            }
        }
    }
}
