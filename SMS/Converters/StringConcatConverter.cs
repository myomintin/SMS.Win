using System;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CodeflexSystems.SMS.Converters
{
    public sealed class StringConcatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string separator = (parameter != null && parameter is string) ? (string)parameter : string.Empty;
            StringBuilder sb = new StringBuilder();
            //int i;

            //for (i = 0; i < values.Length - 1; i++)
            //{
            //    if (values[i] is string)
            //        sb.AppendFormat("{0}{1}", (string)values[i], separator);
            //}
            //if (i < values.Length && values[i] != null)
            //    sb.Append(values[i].ToString());

            foreach (var value in values)
            {
                if (value != null && value != DependencyProperty.UnsetValue)
                    sb.AppendFormat("{0}{1}", value.ToString(), separator);
            }
            if (sb.Length >= separator.Length)
                sb.Remove(sb.Length - separator.Length, separator.Length);
            
            return sb.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
