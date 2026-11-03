using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace CodeflexSystems.SMS.Converters
{
    [ValueConversion(typeof(float?), typeof(string))]
    public sealed class SingleConverter : IValueConverter
    {
        public bool Nullable { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = value as float?;

            if (result.HasValue)
                return result.Value;

            return this.Nullable ? DependencyProperty.UnsetValue : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            float result = 0;
            bool flag = false;

            if (value is string)
                flag = float.TryParse(value as string, NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out result);
            if (flag == false && this.Nullable)
                return null;

            return result;
        }
    }
}
