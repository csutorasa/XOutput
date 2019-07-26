
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using XOutput.Devices.XInput;

namespace XOutput.UI.Converters
{
    /// <summary>
    /// Calculates the color of the elements.
    /// Cannot be used backwards.
    /// </summary>
    public class ColorConverter : IMultiValueConverter
    {
        protected static readonly Brush HighlightBrush = CreateBrush(255, 0, 0);
        protected static readonly Brush HighlightBackBrush = CreateBrush(128, 0, 0);
        protected static readonly Brush HighlightLabelBrush = CreateBrush(255, 255, 255);
        protected static readonly Brush DPadBackBrush = CreateBrush(134, 134, 134);
        protected Dictionary<XInputTypes, Brush> foregroundColors = new Dictionary<XInputTypes, Brush>();
        protected Dictionary<XInputTypes, Brush> backgroundColors = new Dictionary<XInputTypes, Brush>();
        protected Dictionary<XInputTypes, Brush> labelColors = new Dictionary<XInputTypes, Brush>();

        public ColorConverter()
        {
            foregroundColors.Add(XInputTypes.A, CreateBrush(53, 217, 0));
            backgroundColors.Add(XInputTypes.A, CreateBrush(36, 149, 0));
            labelColors.Add(XInputTypes.A, CreateBrush(173, 255, 146));
            foregroundColors.Add(XInputTypes.B, CreateBrush(255, 19, 3));
            backgroundColors.Add(XInputTypes.B, CreateBrush(171, 11, 0));
            labelColors.Add(XInputTypes.B, CreateBrush(255, 161, 155));
            foregroundColors.Add(XInputTypes.X, CreateBrush(14, 82, 255));
            backgroundColors.Add(XInputTypes.X, CreateBrush(0, 50, 176));
            labelColors.Add(XInputTypes.X, CreateBrush(133, 167, 255));
            foregroundColors.Add(XInputTypes.Y, CreateBrush(255, 232, 35));
            backgroundColors.Add(XInputTypes.Y, CreateBrush(217, 195, 0));
            labelColors.Add(XInputTypes.Y, CreateBrush(255, 249, 193));
            foregroundColors.Add(XInputTypes.L1, CreateBrush(255, 248, 248));
            backgroundColors.Add(XInputTypes.L1, CreateBrush(241, 241, 241));
            labelColors.Add(XInputTypes.L1, CreateBrush(187, 187, 187));
            foregroundColors.Add(XInputTypes.R1, CreateBrush(255, 248, 248));
            backgroundColors.Add(XInputTypes.R1, CreateBrush(241, 241, 241));
            labelColors.Add(XInputTypes.R1, CreateBrush(187, 187, 187));
            foregroundColors.Add(XInputTypes.L2, CreateBrush(248, 248, 248));
            backgroundColors.Add(XInputTypes.L2, CreateBrush(187, 187, 187));
            labelColors.Add(XInputTypes.L2, CreateBrush(187, 187, 187));
            foregroundColors.Add(XInputTypes.R2, CreateBrush(248, 248, 248));
            backgroundColors.Add(XInputTypes.R2, CreateBrush(187, 187, 187));
            labelColors.Add(XInputTypes.R2, CreateBrush(187, 187, 187));
            foregroundColors.Add(XInputTypes.L3, CreateBrush(153, 153, 153));
            backgroundColors.Add(XInputTypes.L3, CreateBrush(134, 134, 134));
            foregroundColors.Add(XInputTypes.R3, CreateBrush(153, 153, 153));
            backgroundColors.Add(XInputTypes.R3, CreateBrush(134, 134, 134));
            foregroundColors.Add(XInputTypes.Start, CreateBrush(241, 241, 241));
            backgroundColors.Add(XInputTypes.Start, CreateBrush(217, 217, 217));
            foregroundColors.Add(XInputTypes.Back, CreateBrush(241, 241, 241));
            backgroundColors.Add(XInputTypes.Back, CreateBrush(217, 217, 217));
            foregroundColors.Add(XInputTypes.Home, CreateBrush(202, 202, 202));
            backgroundColors.Add(XInputTypes.Home, CreateBrush(119, 236, 0));
            foregroundColors.Add(XInputTypes.UP, CreateBrush(153, 153, 153));
            backgroundColors.Add(XInputTypes.UP, CreateBrush(134, 134, 134));
            foregroundColors.Add(XInputTypes.DOWN, CreateBrush(153, 153, 153));
            backgroundColors.Add(XInputTypes.DOWN, CreateBrush(134, 134, 134));
            foregroundColors.Add(XInputTypes.LEFT, CreateBrush(153, 153, 153));
            backgroundColors.Add(XInputTypes.LEFT, CreateBrush(134, 134, 134));
            foregroundColors.Add(XInputTypes.RIGHT, CreateBrush(153, 153, 153));
            backgroundColors.Add(XInputTypes.RIGHT, CreateBrush(134, 134, 134));
        }

        /// <summary>
        /// Calculates the color of the elements.
        /// </summary>
        /// <param name="values">XInput type and highlight value</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">XInput type to compare and back/label values</param>
        /// <param name="culture">Ignored</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            XInputTypes? activeType = values[0] as XInputTypes?;
            bool? highlight = values[1] as bool?;
            var parameters = (parameter as string).Split('|');
            bool back = parameters.Length > 1 && parameters[1] == "back";
            bool label = parameters.Length > 1 && parameters[1] == "label";
            if (parameters[0] == "DPAD")
            {
                if (back)
                {
                    if (highlight == true && XInputHelper.Instance.IsDPad(activeType.Value))
                    {
                        return HighlightBackBrush;
                    }
                    return DPadBackBrush;
                }
            }
            else
            {
                var currentType = (XInputTypes)Enum.Parse(typeof(XInputTypes), parameters[0]);
                if (back)
                {
                    if (highlight == true && currentType == activeType)
                    {
                        return HighlightBackBrush;
                    }
                    else if (backgroundColors.ContainsKey(currentType))
                    {
                        return backgroundColors[currentType];
                    }
                }
                else if (label)
                {
                    if (highlight == true && currentType == activeType)
                    {
                        return HighlightLabelBrush;
                    }
                    else if (labelColors.ContainsKey(currentType))
                    {
                        return labelColors[currentType];
                    }
                }
                else
                {
                    if (highlight == true && currentType == activeType)
                    {
                        return HighlightBrush;
                    }
                    else if (foregroundColors.ContainsKey(currentType))
                    {
                        return foregroundColors[currentType];
                    }
                }
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

        protected static Brush CreateBrush(byte r, byte g, byte b)
        {
            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }
    }
}
