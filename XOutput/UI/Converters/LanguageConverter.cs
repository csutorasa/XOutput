using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace XOutput.UI.Converters
{
    /// <summary>
    /// Translates a text.
    /// </summary>
    public class LanguageConverter : IValueConverter
    {
        /// <summary>
        /// Translates a text.
        /// </summary>
        /// <param name="value">translation data</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">Text key</param>
        /// <param name="culture">Ignored</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<string, string> translation = value as Dictionary<string, string>;
            string key = parameter as string;
            return LanguageModel.Translate(translation, key);
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
