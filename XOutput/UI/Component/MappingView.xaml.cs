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
using XOutput.Input.DirectInput;
using XOutput.Input.Mapper;
using XOutput.Input.XInput;
using XOutput.UI.Component;

namespace XOutput.UI.Component
{
    /// <summary>
    /// Interaction logic for MappingView.xaml
    /// </summary>
    public partial class MappingView : UserControl
    {
        protected readonly MappingViewModel viewModel;
        public MappingView(XInputTypes type, MapperData<DirectInputTypes> mapper)
        {
            viewModel = new MappingViewModel(mapper);
            viewModel.XInputType = type;
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
