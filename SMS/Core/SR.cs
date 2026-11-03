using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Resources;

namespace CodeflexSystems.SMS.Core
{
    internal static class SR
    {
        #region Static Fields

        private const string unavailable = "Unavailable";
        private static readonly ResourceManager rm;

        #endregion

        #region Constructor

        static SR()
        {
            rm = new ResourceManager("CodeflexSystems.SMS.Resources.Resources", typeof(SR).Assembly);
        }

        #endregion

        #region Methods

        internal static object GetObject(string id)
        {
            return rm.GetObject(id);
        }

        internal static string GetString(string id)
        {
            string text = rm.GetString(id);

            if (text == null)
            {
                text = unavailable;
                System.Diagnostics.Debug.WriteLine(unavailable);
            }

            return text;
        }

        internal static string GetString(string id, params object[] args)
        {
            string text = rm.GetString(id);

            if (text == null)
            {
                text = unavailable;
                System.Diagnostics.Debug.WriteLine(unavailable);
            }
            else if (args != null && args.Length > 0)
                text = string.Format(CultureInfo.CurrentCulture, text, args);

            return text;
        }

        internal static List<string> GetList(string id, bool sorting = true)
        {
            string resources = rm.GetString(id);

            if (string.IsNullOrEmpty(resources))
                return null;

            var lines = resources.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();

            foreach (var line in lines)
                result.Add(line.XTrim());

            if (sorting)
                return result.OrderBy(m => m).ToList();
            else
                return result;
        }

        internal static List<string> GetAddressTypes()
        {
            return GetList("AddressTypies");
        }

        internal static List<string> GetEmailTypes()
        {
            return GetList("EmailTypies");
        }

        internal static List<string> GetFeeTypes()
        {
            return GetList("FeeTypies");
        }

        internal static List<string> GetPhoneTypes()
        {
            return GetList("PhoneTypies");
        }

        internal static List<string> GetReligions()
        {
            return GetList("Religions");
        }

        internal static List<CurrencyInfo> GetCurrencyInfos()
        {
            string resources = rm.GetString("Currencies");

            if (string.IsNullOrEmpty(resources))
                return null;

            var lines = resources.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<CurrencyInfo>();

            foreach (var line in lines)
            {
                var splits = line.Split('|');

                if (splits.Length == 3)
                    result.Add(new CurrencyInfo
                    {
                        CurrencyCode = splits[0].XTrim(),
                        CurrencyName = splits[1].XTrim(),
                        CurrencySymbol = splits[2].XTrim()
                    });
            }

            return result;
        }

        internal static List<Tuple<string, string, string>> GetCurrencies()
        {
            string resources = rm.GetString("Currencies");

            if (string.IsNullOrEmpty(resources))
                return null;

            var lines = resources.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<Tuple<string, string, string>>();

            foreach (var line in lines)
            {
                var splits = line.Split('|');

                if (splits.Length == 3)
                    result.Add(Tuple.Create(splits[0].XTrim(), splits[1].XTrim(), splits[2].XTrim()));
            }

            return result;
        }

        internal static List<Tuple<string, bool>> GetGuardianTypies()
        {
            string resources = rm.GetString("GuardianTypies");

            if (string.IsNullOrEmpty(resources))
                return null;

            var lines = resources.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<Tuple<string, bool>>();

            foreach (var line in lines)
            {
                var splits = line.Split('|');

                if (splits.Length == 2)
                {
                    bool flag;
                    bool.TryParse(splits[1], out flag);
                    result.Add(new Tuple<string, bool>(splits[0].XTrim(), flag));
                }
                else
                    result.Add(new Tuple<string, bool>(line.XTrim(), true));
            }

            return result;
        }

        internal static List<CountryInfo> GetCountryInfos()
        {
            string resources = rm.GetString("Countries");

            if (string.IsNullOrEmpty(resources))
                return null;

            var lines = resources.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<CountryInfo>();

            foreach (var line in lines.Skip(1))
            {
                var splits = line.Split('|');
                if (splits.Length > 6)
                    result.Add(new CountryInfo
                    {
                        CountryName = splits[5].XTrim(),
                        CountryCode = splits[1].XTrim(),
                        Code = splits[6].XTrim()
                    });
            }

            return result;
        }

        internal static List<Tuple<string, string, string>> GetCountries()
        {
            string resources = rm.GetString("Countries");

            if (string.IsNullOrEmpty(resources))
                return null;

            var lines = resources.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new List<Tuple<string, string, string>>();

            foreach (var line in lines.Skip(1))
            {
                var splits = line.Split('|');
                if (splits.Length > 6)
                    result.Add(new Tuple<string, string, string>(splits[5].XTrim(), splits[1].XTrim(), splits[6].XTrim()));
            }

            return result.OrderBy(m => m.Item1).ToList();
        }

        internal static List<string> GetDateFormats()
        {
            return GetList("DatePatterns", false);
        }

        internal static List<string> GetDateTimeFormats()
        {
            return GetList("DateTimePatterns", false);
        }

        //internal static List<Tuple<string, string>> GetDateTimeFormats()
        //{
        //    string resources = resourceManager.GetString("DateTimePatterns");

        //    if (string.IsNullOrEmpty(resources))
        //        return null;

        //    var lines = resources.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        //    var result = new List<Tuple<string, string>>();

        //    foreach (var line in lines)
        //    {
        //        var splits = line.Split('|');

        //        if (splits.Length == 2)
        //            result.Add(new Tuple<string, string>(splits[0].XTrim(), splits[1].XTrim()));
        //    }

        //    return result;
        //}

        internal static List<string> GetTimeFormats()
        {
            return GetList("TimePatterns", false);
        }

        #endregion

        //#region Throw Exception Methods

        //internal static void IfNullAndNullsAreIllegalThenThrow<T>(object value, string argument)
        //{
        //    if (value == null && default(T) != null)
        //        ThrowArgumentNullException(argument);
        //}

        //internal static void ThrowArgumentException(string message)
        //{
        //    throw new ArgumentException(message);
        //}

        //internal static void ThrowArgumentNullException(string paramName)
        //{
        //    throw new ArgumentNullException(paramName);
        //}

        //internal static void ThrowArgumentOutOfRangeException(string paramName)
        //{
        //    throw new ArgumentOutOfRangeException(paramName);
        //}

        //internal static void ThrowArgumentOutOfRangeException(string paramName, string message)
        //{
        //    throw new ArgumentOutOfRangeException(paramName, message);
        //}

        //internal static void ThrowInvalidOperationException(string message)
        //{
        //    throw new InvalidOperationException(message);
        //}

        //internal static void ThrowArgumentInvalidArrayType()
        //{
        //    throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
        //}

        //internal static void ThrowArgumentOutOfRangeNeedNonNegNum(string argument)
        //{
        //    throw new ArgumentOutOfRangeException(argument, "Non-negative number required.");
        //}

        //internal static void ThrowArgumentRankMultiDimNotSupported()
        //{
        //    throw new ArgumentException("Only single dimensional arrays are supported for the requested action.");
        //}

        //internal static void ThrowArgumentNonZeroLowerBound()
        //{
        //    throw new ArgumentException("The lower bound of target array must be zero.");
        //}

        //internal static void ThrowArgumentArrayPlusOffTooSmall()
        //{
        //    throw new ArgumentException("Destination array is not long enough to copy all the items in the collection. Check array index and length.");
        //}

        //internal static void ThrowInvalidOperationReentrancyNotAllowed(string argument)
        //{
        //    throw new InvalidOperationException(string.Format(
        //        "Cannot change {0} during a CollectionChanged event.", argument));
        //}

        //internal static void ThrowWrongTypeArgumentException(object value, Type type, string argument)
        //{
        //    throw new ArgumentException(string.Format(
        //        "The value '{0}' is not of type '{1}' and cannot be used in this generic collection.",
        //        value, type), argument);
        //}

        //#endregion
    }
}
