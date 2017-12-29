using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace XOutput.UI.Converters
{
    /// <summary>
    /// Inverts a boolean value.
    /// </summary>
    public class BoolInverterConverter : IValueConverter
    {
        /// <summary>
        /// Inverts the bool value.
        /// </summary>
        /// <param name="value">Boolean value to convert</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">Ignored</param>
        /// <param name="culture">Ignored</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Invert(value as bool?);
        }

        /// <summary>
        /// Inverts the bool value.
        /// </summary>
        /// <param name="value">Boolean value to convert</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">Ignored</param>
        /// <param name="culture">Ignored</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Invert(value as bool?);
        }

        protected bool Invert(bool? b)
        {
            if (b.HasValue)
            {
                return !(b.Value);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
