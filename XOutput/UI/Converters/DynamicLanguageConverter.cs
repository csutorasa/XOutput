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
    /// </summary>
    public class DynamicLanguageConverter : IMultiValueConverter
    {
        /// <summary>
        /// Translates a text.
        /// </summary>
        /// <param name="values">Ignored</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">Text key</param>
        /// <param name="culture">Ignored</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<string, string> translation = values[0] as Dictionary<string, string>;
            string key = values[1] as string;
            if(translation == null || key == null || !translation.ContainsKey(key))
            {
                return key;
            }
            return translation[key];
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
