using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AwesomeOverlay.PropertyConverters
{
    class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility FalseValue { get; set; }
        public bool Inverted { get; set; }

        public BooleanToVisibilityConverter()
        { 
            FalseValue = Visibility.Hidden;
            Inverted = false;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Inverted)
                return !(bool)value ? Visibility.Visible : FalseValue;
            else
                return (bool)value ? Visibility.Visible : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
