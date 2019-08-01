using System;
using System.Windows;

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
    }
}
