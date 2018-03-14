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
using XOutput.Input.XInput;

namespace XOutput.UI.Converters
{
    /// <summary>
    /// Translates a text.
    /// </summary>
    public class ColorConverter : IMultiValueConverter
    {
        protected Dictionary<XInputTypes, Brush> foregroundColors = new Dictionary<XInputTypes, Brush>();
        protected Dictionary<XInputTypes, Brush> backgroundColors = new Dictionary<XInputTypes, Brush>();

        public ColorConverter()
        {
            foregroundColors.Add(XInputTypes.A, CreateData(53, 217, 0));
            backgroundColors.Add(XInputTypes.A, CreateData(36, 149, 0));
            foregroundColors.Add(XInputTypes.B, CreateData(255, 19, 3));
            backgroundColors.Add(XInputTypes.B, CreateData(171, 11, 0));
            foregroundColors.Add(XInputTypes.X, CreateData(14, 82, 255));
            backgroundColors.Add(XInputTypes.X, CreateData(0, 50, 176));
            foregroundColors.Add(XInputTypes.Y, CreateData(255, 232, 35));
            backgroundColors.Add(XInputTypes.Y, CreateData(217, 195, 0));
            foregroundColors.Add(XInputTypes.L1, CreateData(255, 248, 248));
            backgroundColors.Add(XInputTypes.L1, CreateData(241, 241, 241));
            foregroundColors.Add(XInputTypes.R1, CreateData(255, 248, 248));
            backgroundColors.Add(XInputTypes.R1, CreateData(241, 241, 241));
            foregroundColors.Add(XInputTypes.L2, CreateData(248, 248, 248));
            backgroundColors.Add(XInputTypes.L2, CreateData(187, 187, 187));
            foregroundColors.Add(XInputTypes.R2, CreateData(248, 248, 248));
            backgroundColors.Add(XInputTypes.R2, CreateData(187, 187, 187));
            foregroundColors.Add(XInputTypes.L3, CreateData(153, 153, 153));
            backgroundColors.Add(XInputTypes.L3, CreateData(134, 134, 134));
            foregroundColors.Add(XInputTypes.R3, CreateData(153, 153, 153));
            backgroundColors.Add(XInputTypes.R3, CreateData(134, 134, 134));
            foregroundColors.Add(XInputTypes.Start, CreateData(241, 241, 241));
            backgroundColors.Add(XInputTypes.Start, CreateData(217, 217, 217));
            foregroundColors.Add(XInputTypes.Back, CreateData(241, 241, 241));
            backgroundColors.Add(XInputTypes.Back, CreateData(217, 217, 217));
            foregroundColors.Add(XInputTypes.Home, CreateData(202, 202, 202));
            backgroundColors.Add(XInputTypes.Home, CreateData(119, 236, 0));


        }

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
            XInputTypes? activeType = values[0] as XInputTypes?;
            bool? highlight = values[1] as bool?;
            var parameters = (parameter as string).Split('|');
            var currentType = (XInputTypes)Enum.Parse(typeof(XInputTypes), parameters[0]);
            bool back = parameters.Length > 1 && parameters[1] == "back";
            if (back)
            {
                if (highlight == true && currentType == activeType)
                    return new SolidColorBrush(Color.FromRgb(128, 0, 0));
                else if (backgroundColors.ContainsKey(currentType))
                    return backgroundColors[currentType];
            }
            else
            {
                if (highlight == true && currentType == activeType)
                    return new SolidColorBrush(Color.FromRgb(255, 0, 0));
                else if (foregroundColors.ContainsKey(currentType))
                    return foregroundColors[currentType];
            }
            return new SolidColorBrush(Colors.Black);
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

        protected Brush CreateData(byte r, byte g, byte b)
        {
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }
    }
}
