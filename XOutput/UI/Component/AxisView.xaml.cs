using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XOutput.UI.Component;

namespace XOutput.UI.Component
{
    /// <summary>
    /// Interaction logic for AxisView.xaml
    /// </summary>
    public partial class AxisView : UserControl
    {
        public int Value { get { return viewModel.Value; } set { viewModel.Value = value; } }
        public Enum Type { get { return viewModel.Type; } }

        protected readonly AxisModel viewModel;

        public AxisView(Enum type, int max = 1000)
        {
            viewModel = new AxisModel();
            viewModel.Type = type;
            viewModel.Label = type.ToString();
            viewModel.Max = max;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
