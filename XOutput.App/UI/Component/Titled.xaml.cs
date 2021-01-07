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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XOutput.App.UI.Component
{
    [ContentPropertyAttribute("Control")]
    public partial class Titled : UserControl
    {
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(TitleProperty), typeof(string), typeof(Titled));
        public string Title
        {
            get => GetValue(TitleProperty) as string;
            set { SetValue(TitleProperty, value); }
        }
        public readonly static DependencyProperty ControlProperty = DependencyProperty.Register(nameof(ControlProperty), typeof(FrameworkElement), typeof(Titled));
        public FrameworkElement Control
        {
            get => GetValue(ControlProperty) as FrameworkElement;
            set { SetValue(ControlProperty, value); }
        }
        public TranslationModel Translation => TranslationModel.Instance;

        public Titled()
        {
            InitializeComponent();
        }
    }
}
