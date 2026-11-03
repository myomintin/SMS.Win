using System;
using System.Windows.Data;

namespace SMS.Converters
{
    [ValueConversion(typeof(TimeSpan?), typeof(DateTime?))]
    public sealed class TimeOfDayToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                return new DateTime(((TimeSpan)value).Ticks);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime)
            {
                DateTime dt = (DateTime)value;

                return new TimeSpan(dt.Hour, dt.Minute, dt.Second);
            }

            return value;
        }
    }
}
