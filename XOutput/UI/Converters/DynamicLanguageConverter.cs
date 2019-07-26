using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace XOutput.UI.Converters
{
    /// <summary>
    /// Translates a text.
    /// Cannot be used backwards.
    /// </summary>
    public class DynamicLanguageConverter : IMultiValueConverter
    {
        /// <summary>
        /// Translates a text.
        /// </summary>
        /// <param name="values">translation values and text to translate</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">Ignored</param>
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
            else if (values[1] is string)
            {
                key = values[1] as string;
            }
            else if (values[1] is bool)
            {
                key = (bool)values[1] ? "True" : "False";
            }
            else if (values[1] is sbyte || values[1] is byte || values[1] is char || values[1] is short || values[1] is ushort || values[1] is int || values[1] is uint || values[1] is long || values[1] is ulong || values[1] is decimal)
            {
                return values[1].ToString();
            }
            else
            {
                key = values[1] as string;
            }
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
            if (translations == null || key == null || !translations.ContainsKey(key))
            {
                return null;
            }
            return translations[key];
        }
    }
}
