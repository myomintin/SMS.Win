using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace CodeflexSystems.SMS.Converters
{
    [ValueConversion(typeof(decimal?), typeof(string))]
    public sealed class CurrencyConverter : IValueConverter
    {
        public bool Nullable { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = value as decimal?;

            if (result.HasValue)
                return result.Value.ToString("C");

            return this.Nullable ? DependencyProperty.UnsetValue : decimal.Zero.ToString("C");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal result = 0;
            bool flag = false;

            if (value is string)
                flag = decimal.TryParse(value.ToString(), NumberStyles.Any, Thread.CurrentThread.CurrentCulture, out result);
            if (flag == false && this.Nullable)
                return null;

            return result;
        }
    }
}
