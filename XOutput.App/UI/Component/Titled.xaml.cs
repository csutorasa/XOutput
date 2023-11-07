using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace XOutput.App.UI.Component
{
    [ContentProperty("Control")]
    public partial class Titled : UserControl
    {
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(Titled));
        public string Title
        {
            get => GetValue(TitleProperty) as string;
            set { SetValue(TitleProperty, value); }
        }
        public readonly static DependencyProperty ControlProperty = DependencyProperty.Register(nameof(Control), typeof(FrameworkElement), typeof(Titled));
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
