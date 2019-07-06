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
    /// Interaction logic for InputView.xaml
    /// </summary>
    public partial class InputView : UserControl, IViewBase<InputViewModel, InputModel>
    {
        protected readonly InputViewModel viewModel;
        public InputViewModel ViewModel => viewModel;

        public InputView(InputViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
        private void OpenClick(object sender, RoutedEventArgs e)
        {
            viewModel.Edit();
        }
    }
}
