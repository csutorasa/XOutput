using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace XOutput.UI.Converters
{
    /// <summary>
    /// Compares enum value and converts to visibility.
    /// Cannot be used backwards.
    /// </summary>
    public class EqualsToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Compares enum value and converts to visibility.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">other value</param>
        /// <param name="culture">Ignored</param>
        /// <returns>If the values equal</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Intentionally not implemented.
        /// </summary>
        /// <param name="value">Ignored</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">Ignored</param>
        /// <param name="culture">Ignored</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
