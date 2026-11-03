using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CodeflexSystems.SMS.Converters
{
    public sealed class ByteArrayToImageConverter : IValueConverter
    {
        private ImageSourceConverter converter = new ImageSourceConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                if (value is string)
                    return new BitmapImage(new Uri(value as string, UriKind.RelativeOrAbsolute));
                else if (value is ImageSource)
                    return value as ImageSource;
                else
                    return converter.ConvertFrom(value) as ImageSource;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
