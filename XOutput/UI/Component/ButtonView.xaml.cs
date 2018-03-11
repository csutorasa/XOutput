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
using XOutput.Input;
using XOutput.UI.Component;

namespace XOutput.UI.Component
{
    /// <summary>
    /// Interaction logic for ButtonView.xaml
    /// </summary>
    public partial class ButtonView : UserControl, IUpdatableView, IViewBase<ButtonViewModel, ButtonModel>
    {
        protected readonly ButtonViewModel viewModel;
        public ButtonViewModel ViewModel => viewModel;

        public ButtonView(Enum type)
        {
            viewModel = new ButtonViewModel(type);
            DataContext = viewModel;
            InitializeComponent();
        }

        public void UpdateValues(IDevice device)
        {
            viewModel.UpdateValues(device);
        }
    }
}
