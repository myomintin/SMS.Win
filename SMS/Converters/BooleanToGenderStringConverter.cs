using System;
using System.Windows.Data;

namespace CodeflexSystems.SMS.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public sealed class BooleanToGenderStringConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = value as bool?;

            if (result.HasValue && result.Value)
                return this.Inverse ? "Female" : "Male";

            return this.Inverse ? "Male" : "Female";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool flag = false;

            if (value is string)
                flag = (value as string).ToLower() == "male";

            return this.Inverse ? !flag : flag;
        }
    }

    //[ValueConversion(typeof(bool), typeof(string))]
    //public sealed class BooleanToGenderStringConverter : IValueConverter
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
    //            return flag ? "Female" : "Male";
    //        else
    //            return flag ? "Male" : "Female";
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        bool flag = false;
    //        bool inverse = (parameter as string) == "inverse";

    //        if (value is string)
    //        {
    //            var str = (value as string).ToLower();
    //            flag = str == "male";
    //        }

    //        return inverse ? !flag : flag;
    //    }
    //}
}
