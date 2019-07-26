using System.Windows;
using System.Windows.Controls;

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
