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
            Dictionary<string, string> translations = values[0] as Dictionary<string, string>;
            string key;
            if (values[1] is Enum)
            {
                key = values[1].GetType().Name + "." + values[1].ToString();
                return getTranslation(translations, key) ?? values[1].ToString();
            }
            key = values[1] as string;
            return getTranslation(translations, key) ?? key;
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

        protected string getTranslation(Dictionary<string, string> translations, string key)
        {
            if(translations == null || key == null || !translations.ContainsKey(key))
            {
                return null;
            }
            return translations[key];
        }
    }
}
