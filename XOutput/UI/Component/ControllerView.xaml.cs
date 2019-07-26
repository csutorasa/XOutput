using System;
using System.Windows;
using System.Windows.Controls;

namespace XOutput.UI.Component
{
    /// <summary>
    /// Interaction logic for ControllerView.xaml
    /// </summary>
    public partial class ControllerView : UserControl, IViewBase<ControllerViewModel, ControllerModel>
    {
        public event Action<ControllerView> RemoveClicked;

        protected readonly ControllerViewModel viewModel;
        public ControllerViewModel ViewModel => viewModel;

        public ControllerView(ControllerViewModel viewModel)
        {
            this.viewModel = viewModel;
            DataContext = viewModel;
            InitializeComponent();
        }
        private void OpenClick(object sender, RoutedEventArgs e)
        {
            viewModel.Edit();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.StartStop();
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            RemoveClicked?.Invoke(this);
        }
    }
}
