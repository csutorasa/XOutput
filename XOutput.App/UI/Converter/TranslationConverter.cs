using System;
using System.Globalization;
using System.Windows.Data;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI.Converter
{
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
