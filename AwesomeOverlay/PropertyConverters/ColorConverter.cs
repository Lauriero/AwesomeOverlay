using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AwesomeOverlay.PropertyConverters
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null)
                return null;

            return ((SolidColorBrush)value).Color;
        }
    }
}
