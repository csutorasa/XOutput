using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace XOutput.UI.Converters
{
    /// <summary>
    /// Multi converter class.
    /// </summary>
    public class MultiConverter : List<IValueConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object returnValue = value;
            foreach (var converter in this)
            {
                returnValue = converter.Convert(returnValue, targetType, parameter, culture);
            }
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
