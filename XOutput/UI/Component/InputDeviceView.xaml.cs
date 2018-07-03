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

namespace XOutput.UI.Component
{
    /// <summary>
    /// Interaction logic for InputDeviceView.xaml
    /// </summary>
    public partial class InputDeviceView : UserControl, IViewBase<InputDeviceViewModel, InputDeviceModel>
    {
        protected readonly InputDeviceViewModel viewModel;
        public InputDeviceViewModel ViewModel => viewModel;

        public InputDeviceView(InputDeviceViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
