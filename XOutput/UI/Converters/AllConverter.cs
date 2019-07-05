using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace XOutput.UI.Converters
{
    /// <summary>
    /// Translates a text.
    /// Cannot be used backwards.
    /// </summary>
    public class AllConverter : IMultiValueConverter
    {
        /// <summary>
        /// Checks if all of the parameters are true.
        /// </summary>
        /// <param name="values">values to check</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">Ignored</param>
        /// <param name="culture">Ignored</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var v in values)
            {
                if (!(v is bool) || !((bool)v))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Intentionally not implemented.
        /// </summary>
        /// <param name="value">Ignored</param>
        /// <param name="targetTypes">Ignored</param>
        /// <param name="parameter">Ignored</param>
        /// <param name="culture">Ignored</param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
