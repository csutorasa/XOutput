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
    /// Interaction logic for ButtonView.xaml
    /// </summary>
    public partial class ButtonView : UserControl
    {
        public bool Value { get { return viewModel.Value; }  set { viewModel.Value = value; } }
        public Enum Type { get { return viewModel.Type; } }

        protected readonly ButtonModel viewModel;

        public ButtonView(Enum type)
        {
            viewModel = new ButtonModel();
            viewModel.Type = type;
            viewModel.Label = type.ToString();
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
