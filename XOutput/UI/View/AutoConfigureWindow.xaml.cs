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
using System.Windows.Shapes;
using System.Windows.Threading;
using XOutput.Input;
using XOutput.Input.Mapper;
using XOutput.Input.XInput;

namespace XOutput.UI.View
{
    /// <summary>
    /// Interaction logic for AutoConfigureWindow.xaml
    /// </summary>
    public partial class AutoConfigureWindow : Window
    {
        private readonly AutoConfigureViewModel viewModel;

        public AutoConfigureWindow(GameController controller, XInputTypes valueToRead)
        {
            viewModel = new AutoConfigureViewModel(controller, valueToRead);
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel.Initialize();
        }
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            viewModel.SaveValues();
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
