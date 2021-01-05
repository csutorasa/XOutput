using System;
using System.Globalization;
using System.Windows.Data;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.UI.Converter
{
    public class DynamicTranslationConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
            {
                throw new ArgumentException("There must be 2 values");
            }
            string key;
            if (values[1] is string)
            {
                key = values[1] as string;
            }
            else
            {
                throw new ArgumentException("Key is not a string");
            }
            return ApplicationContext.Global.Resolve<TranslationService>().Translate(key);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
