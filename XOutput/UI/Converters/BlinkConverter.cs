using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using XOutput.Devices.XInput;

namespace XOutput.UI.Converters
{
    /// <summary>
    /// Decides if the element should be visible or not.
    /// Cannot be used backwards.
    /// </summary>
    public class BlinkConverter : IMultiValueConverter
    {
        /// <summary>
        /// Decides if the element should be visible or not.
        /// </summary>
        /// <param name="values">XInput type and highlight value</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">XInput type to compare and back value</param>
        /// <param name="culture">Ignored</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            XInputTypes? activeType = values[0] as XInputTypes?;
            bool? highlight = values[1] as bool?;
            var parameters = (parameter as string).Split('|');
            var currentType = (XInputTypes)Enum.Parse(typeof(XInputTypes), parameters[0]);
            bool back = parameters.Length > 1 && parameters[1] == "back";
            if (back)
            {
                if (currentType == activeType)
                {
                    return highlight == true ? Visibility.Visible : Visibility.Hidden;
                }
                return Visibility.Hidden;
            }
            else
            {
                if (currentType == activeType)
                {
                    return highlight == true ? Visibility.Hidden : Visibility.Visible;
                }
                return Visibility.Hidden;
            }
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
