using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace XOutput.UI.Component
{
    public class NumericTextBox : TextBox
    {

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(decimal?), typeof(NumericTextBox));
        public decimal? Minimum
        {
            get { return (decimal?)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(decimal?), typeof(NumericTextBox));
        public decimal? Maximum
        {
            get { return (decimal?)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(decimal?), typeof(NumericTextBox), new PropertyMetadata(ValueChanged));
        public decimal? Value
        {
            get { return (decimal?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        protected static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericTextBox numericTextBox = d as NumericTextBox;
            if (numericTextBox.Value.HasValue)
                numericTextBox.Text = decimal.Round(numericTextBox.Value.Value).ToString();
            else
                numericTextBox.Text = "";
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            decimal value;
            if (string.IsNullOrEmpty(Text))
            {
                Value = null;
            }
            else
            {
                if (decimal.TryParse(Text, out value))
                {
                    if (Minimum.HasValue && value < Minimum)
                        Value = Minimum.Value;
                    else if (Maximum.HasValue && value > Maximum)
                        Value = Maximum.Value;
                    else
                        Value = value;
                }
                else
                {
                    if (Minimum.HasValue)
                        Value = Minimum.Value;
                    else if (Maximum.HasValue)
                        Value = Maximum.Value;
                    else
                        Value = Value;
                }
            }
            base.OnTextChanged(e);
        }
    }
}
