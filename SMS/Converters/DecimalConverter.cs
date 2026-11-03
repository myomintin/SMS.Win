using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;

namespace SMS.Converters
{
    [ValueConversion(typeof(decimal?), typeof(string))]
    public sealed class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal result = 0;

            if (value is string)
                decimal.TryParse(value as string, NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out result);

            return result;
        }
    }
}
