
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AwesomeOverlay.PropertyConverters
{
    public class UriToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null)
                return null;

            Uri source = value as Uri;

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new FileStream(source.AbsolutePath, FileMode.Open);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            image.StreamSource.Dispose();

            return image;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
