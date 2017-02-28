using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.UI.Component;

namespace XOutput.UI.View
{
    public class AutoConfigureModel : ModelBase
    {
        private string labelText;
        public string LabelText
        {
            get { return labelText; }
            set { if (labelText != value) { labelText = value; OnPropertyChanged(nameof(LabelText)); } }
        }
        private bool isAuto = true;
        public bool IsAuto
        {
            get { return isAuto; }
            set { if (isAuto != value) { isAuto = value; OnPropertyChanged(nameof(IsAuto)); if (value) { MaxType = null; } } }
        }
        private Enum maxType;
        public Enum MaxType
        {
            get { return maxType; }
            set { if (maxType != value) { maxType = value; OnPropertyChanged(nameof(MaxType)); } }
        }
    }
}
