using System;
using System.Windows.Data;

namespace CodeflexSystems.SMS.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public sealed class BooleanToYesNoConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? result = value as bool?;

            if (result.HasValue && result.Value)
                return this.Inverse ? "No" : "Yes";

            return this.Inverse ? "Yes" : "No";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool flag = false;

            if (value is string)
                flag = (value as string).ToLower() == "yes";

            return this.Inverse ? !flag : flag;
        }
    }

    //[ValueConversion(typeof(bool), typeof(string))]
    //public sealed class BooleanToYesNoConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        bool flag = false;

    //        if (value is bool)
    //            flag = (bool)value;
    //        else if (value is bool?)
    //        {
    //            bool? nullable = (bool?)value;
    //            flag = nullable.HasValue ? nullable.Value : false;
    //        }

    //        bool inverse = (parameter as string) == "inverse";

    //        if (inverse)
    //            return flag ? "No" : "Yes";
    //        else
    //            return flag ? "Yes" : "No";
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        bool flag = false;
    //        bool inverse = (parameter as string) == "inverse";

    //        if (value is string)
    //        {
    //            var str = (value as string).ToLower();
    //            flag = str == "yes";
    //        }

    //        return inverse ? !flag : flag;
    //    }
    //}
}
