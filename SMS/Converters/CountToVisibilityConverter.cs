using System;
using System.Windows;
using System.Windows.Data;

namespace CodeflexSystems.SMS.Converters
{
    public sealed class CountToVisibilityConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = value as int?;

            if (result.HasValue && result.Value > 0)
                return this.Inverse ? Visibility.Collapsed : Visibility.Visible;

            return this.Inverse ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //public sealed class CountToVisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        //bool flag = false;
    //        //bool inverse = (parameter as string) == "inverse";

    //        //if (value is int || value is long)
    //        //{
    //        //    long result = (int)value;
    //        //    flag = result > 0;
    //        //}
    //        //else if (value is int? || value is long?)
    //        //{
    //        //    long? result = (int?)value;
    //        //    flag = result.HasValue && result.Value > 0;
    //        //}

    //        int? result = (int?)value;
    //        bool flag = result.HasValue && result.Value > 0;
    //        bool inverse = (parameter as string) == "inverse";

    //        if (inverse)
    //            return flag ? Visibility.Collapsed : Visibility.Visible;
    //        else
    //            return flag ? Visibility.Visible : Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
