using System;
using System.Collections.Generic;
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
    /// Interaction logic for MappingView.xaml
    /// </summary>
    public partial class MappingView : UserControl, IViewBase<MappingViewModel, MappingModel>
    {
        protected readonly MappingViewModel viewModel;
        public MappingViewModel ViewModel => viewModel;

        public MappingView(MappingViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        public void Refresh()
        {
            viewModel.Refresh();
        }

        private void ConfigureClick(object sender, RoutedEventArgs e)
        {
            viewModel.Configure();
            Refresh();
        }

        private void InvertClick(object sender, RoutedEventArgs e)
        {
            viewModel.Invert();
        }
    }
}
