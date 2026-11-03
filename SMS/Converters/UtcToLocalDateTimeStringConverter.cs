using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SMS.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public sealed class UtcToLocalDateTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;

            DateTime? dt = value as DateTime?;
            if (dt.HasValue)
            {
                if (parameter != null && parameter is string)
                {
                    try
                    {
                        return dt.Value.ToLocalTime().ToString(parameter as string);
                    }
                    catch
                    {
                        return dt.Value.ToLocalTime().ToString();
                    }
                }
                else
                    return dt.Value.ToLocalTime().ToString();
            }
            else
                return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null && !(value is string))
                return DependencyProperty.UnsetValue;

            DateTime dt;
            var flag = false;

            try
            {
                if (parameter != null && parameter is string)
                    flag = DateTime.TryParseExact(value as string, parameter as string, culture, System.Globalization.DateTimeStyles.None, out dt);
                else
                    flag = DateTime.TryParse(value as string, out dt);

                if (flag)
                    return dt.ToUniversalTime();
                else
                    return DependencyProperty.UnsetValue;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
