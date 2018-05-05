using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XOutput.UI.Windows
{
    /// <summary>
    /// Interaction logic for AutoConfigureWindow.xaml
    /// </summary>
    public partial class DiagnosticsWindow : Window, IViewBase<DiagnosticsViewModel, DiagnosticsModel>
    {
        private readonly DiagnosticsViewModel viewModel;
        public DiagnosticsViewModel ViewModel => viewModel;

        public DiagnosticsWindow(DiagnosticsViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void WindowClosed(object sender, EventArgs e)
        {

        }
    }
}
