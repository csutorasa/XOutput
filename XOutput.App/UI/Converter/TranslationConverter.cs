using System;
using System.Globalization;
using System.Windows.Data;
using XOutput.DependencyInjection;

namespace XOutput.App.UI.Converter
{
    /// <summary>
    /// Converts a text based on the language, and updates when the language changes.
    /// </summary>
    /// <example>
    /// <TextBlock Text="{Binding Translation.Language, Converter={StaticResource Translator}, ConverterParameter='Test.Key' />
    /// </example>
    public class TranslationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string key;
            if (parameter is string)
            {
                key = parameter as string;
            }
            else
            {
                throw new ArgumentException("Parameter is not a string");
            }
            return ApplicationContext.Global.Resolve<TranslationService>().Translate(key);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
