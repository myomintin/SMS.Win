using CodeflexSystems.Library.Extensions;
using SMS.Core;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Extensions
{
    public static class ModelExtension
    {
        public static bool IsSchool(this FeeType value)
        {
            return value.FeeTypeName == "School";
        }

        public static bool IsFerry(this FeeType value)
        {
            return value.FeeTypeName == "Ferry";
        }

        public static bool IsAdmission(this FeeType value)
        {
            return value.FeeTypeName == "Admission";
        }

        public static T GetDefault<T>(this IEnumerable<T> source)
        {
            return source.FirstOrDefault();
        }

        public static Country GetDefault(this IEnumerable<Country> source, string countryCode)
        {
            if (string.IsNullOrEmpty(countryCode))
                throw new ArgumentNullException("countryCode");

            var value = source.FirstOrDefault(m => m.CountryCode == countryCode);

            return value ?? source.FirstOrDefault();
        }

        public static ObservableCollection<AcademicYear> WithDummy(this IEnumerable<AcademicYear> source, string name = "<Unknown>")
        {
            var list = source.ToObservableCollection();

            list.Insert(0, new AcademicYear { AcademicYearName = name });

            return list;
        }

        public static ObservableCollection<Country> WithDummy(this IEnumerable<Country> source, string name = "<Unknown>")
        {
            var list = source.ToObservableCollection();

            list.Insert(0, new Country { CountryName = name });

            return list;
        }

        public static ObservableCollection<Currency> WithDummy(this IEnumerable<Currency> source, string name = "<Unknown>")
        {
            var list = source.ToObservableCollection();

            list.Insert(0, new Currency { CurrencyName = name });

            return list;
        }

        public static ObservableCollection<Religion> WithDummy(this IEnumerable<Religion> source, string name = "<Unknown>")
        {
            var list = source.ToObservableCollection();

            list.Insert(0, new Religion { ReligionName = name });

            return list;
        }

        public static ObservableCollection<School> WithDummy(this IEnumerable<School> source, string name = "<Unknown>")
        {
            var list = source.ToObservableCollection();

            list.Insert(0, new School { SchoolName = name });

            return list;
        }

        public static decimal GetDiscountAmount(this StudentDiscount value, decimal defaultValue = 0)
        {
            if (value == null)
                return defaultValue;

            return value.DiscountAmount;
        }

        public static decimal? GetDiscountAmountNullable(this StudentDiscount value, decimal? defaultValue = null)
        {
            if (value == null)
                return defaultValue;

            return value.DiscountAmount;
        }

        //public static float GetDiscountPercentage(this StudentDiscount value, float defaultValue = 0)
        //{
        //    if (value == null)
        //        return defaultValue;

        //    return value.DiscountPercentage;
        //}

        public static decimal GetDiscountPercentage(this StudentDiscount value, decimal defaultValue = 0)
        {
            if (value == null)
                return defaultValue;

            return value.ComputeDiscountPercentage(defaultValue);
        }

        public static decimal GetFee(this StudentFerryFee value, decimal defaultValue = 0)
        {
            if (value == null)
                return defaultValue;

            return value.Fee;
        }

        public static decimal GetFee(this Section value, decimal defaultValue = 0)
        {
            if (value == null || value.Fee.HasValue == false)
                return defaultValue;

            return value.Fee.Value;
        }

        public static string ToAgeString(this DateTime? dt, DateTime td)
        {
            if (dt.HasValue)
                return ToAgeString(dt.Value, td);

            return null;
        }

        public static string ToAgeString(this DateTime dt, DateTime td)
        {
            var age = new DateTime((td - dt).Ticks);

            return string.Format("{0}-{1}", age.Year - 1, age.Month - 1);
        }

        public static string ToDateString(this DateTime? dt)
        {
            if (dt.HasValue)
                return dt.Value.ToString("d/M/yyyy");

            return null;
        }

        public static string ToDateString(this DateTime dt)
        {
            return dt.ToString("d/M/yyyy");
        }

        public static string ToTimeString(this TimeSpan? ts)
        {
            if (ts.HasValue)
                return new DateTime(ts.Value.Ticks).ToString(DateTimeFormatInfo.CurrentInfo.ShortTimePattern);

            return null;
        }

        public static string ToTimeString(this TimeSpan ts)
        {
            return new DateTime(ts.Ticks).ToString(DateTimeFormatInfo.CurrentInfo.ShortTimePattern);
        }

        public static string ToAddressString(this IEnumerable<PersonAddress> source, int index, bool all =true)
        {
            if (source == null)
                return null;

            var getAddress = GetAddress(index);
            if (all)
                return ", ".Combine(source.Select(m => getAddress(m.Address)));
            else
            {
                if (source.Any(m => m.IsDefault))
                    return ", ".Combine(source.Where(m => m.IsDefault).Select(m => getAddress(m.Address)));

                var item = source.FirstOrDefault();
                return item == null ? null : getAddress(item.Address);
            }
        }

        public static string ToEmailString(this IEnumerable<Email> source, bool all = true)
        {
            if (source == null)
                return null;

            if (all)
                return ", ".Combine(source.Select(m => m.EmailAddress));
            else
            {
                if (source.Any(m => m.IsDefault))
                    return ", ".Combine(source.Where(m => m.IsDefault).Select(m => m.EmailAddress));

                var item = source.FirstOrDefault();
                return item == null ? null : item.EmailAddress;
                //return source.Select(m => m.EmailAddress).FirstOrDefault();
            }
        }

        public static string ToPhoneString(this IEnumerable<Phone> source, bool all = true)
        {
            if (source == null)
                return null;

            if (all)
                return ", ".Combine(source.Select(m => "-".Combine(m.PhoneNumber, m.Extension)));
            else
            {
                if (source.Any(m => m.IsDefault))
                    return ", ".Combine(source.Where(m => m.IsDefault)
                        .Select(m => "-".Combine(m.PhoneNumber, m.Extension)));

                var item = source.FirstOrDefault();
                return item == null ? null : "-".Combine(item.PhoneNumber, item.Extension);
            }
        }

        public static string ToGuaridanString(this IEnumerable<PersonGuardian> source, List<Guid> guardianTypeIds, int index)
        {
            if (source == null)
                return null;

            var getGuardianName = GetGuardianName(index);
            if (guardianTypeIds == null || guardianTypeIds.Count == 0)
                return ", ".Combine(source.Select(m => getGuardianName(m.Guardian)));

            return ", ".Combine(source.Where(m => guardianTypeIds.Contains(m.GuardianTypeId)).Select(m => getGuardianName(m.Guardian)));
        }

        public static IOrderedQueryable<AcademicYear> Ordered(this IQueryable<AcademicYear> query)
        {
            return query
                .OrderBy(m => m.AcademicYearName);
        }

        public static IOrderedQueryable<Address> Ordered(this IQueryable<Address> query)
        {
            return query
                .OrderBy(m => m.Line1)
                .ThenBy(m => m.City);
        }

        public static IOrderedQueryable<AddressType> Ordered(this IQueryable<AddressType> query)
        {
            return query
                .OrderBy(m => m.AddressTypeName);
        }

        public static IOrderedQueryable<Class> Ordered(this IQueryable<Class> query)
        {
            return query
                .OrderBy(m => m.ClassName);
        }

        public static IOrderedQueryable<Course> Ordered(this IQueryable<Course> query)
        {
            return query
                .OrderBy(m => m.CourseName);
        }

        public static IOrderedQueryable<Currency> Ordered(this IQueryable<Currency> query)
        {
            return query
                .OrderBy(m => m.CurrencyName);
        }

        public static IOrderedQueryable<EmailType> Ordered(this IQueryable<EmailType> query)
        {
            return query
                .OrderBy(m => m.EmailTypeName);
        }

        public static IOrderedQueryable<FeeType> Ordered(this IQueryable<FeeType> query)
        {
            return query
                .OrderBy(m => m.FeeTypeName);
        }

        public static IOrderedQueryable<Guardian> Ordered(this IQueryable<Guardian> query)
        {
            return query
                .OrderBy(m => m.GuardianName);
        }

        public static IOrderedQueryable<GuardianType> Ordered(this IQueryable<GuardianType> query)
        {
            return query
                .OrderBy(m => m.GuardianTypeName);
        }

        public static IOrderedQueryable<PhoneType> Ordered(this IQueryable<PhoneType> query)
        {
            return query
                .OrderBy(m => m.PhoneTypeName);
        }

        public static IQueryable<Receipt> Ordered(this IQueryable<Receipt> query)
        {
            return query
                .OrderBy(m => m.ReceiptCode)
                .OrderBy(m => m.ReceiptNo)
                .ThenBy(m => m.CreatedDate);
        }

        public static IOrderedQueryable<Religion> Ordered(this IQueryable<Religion> query)
        {
            return query
                .OrderBy(m => m.ReligionName);
        }

        public static IOrderedQueryable<School> Ordered(this IQueryable<School> query)
        {
            return query
                .OrderBy(m => m.SchoolName);
        }

        public static IOrderedQueryable<Section> Ordered(this IQueryable<Section> query)
        {
            return query
                .OrderBy(m => m.SectionName);
        }

        public static IOrderedQueryable<Student> Ordered(this IQueryable<Student> query)
        {
            return query
                .OrderBy(m => m.StudentName)
                .ThenBy(m => m.OtherName);
        }

        public static IOrderedQueryable<StudentDiscount> Ordered(this IQueryable<StudentDiscount> query)
        {
            return query
                .OrderBy(m => m.StudentSection.Student.StudentName)
                .ThenBy(m => m.StudentSection.Student.OtherName)
                .ThenBy(m => m.StudentSection.Section.SectionName);
        }

        public static IOrderedQueryable<StudentFerryFee> Ordered(this IQueryable<StudentFerryFee> query)
        {
            return query
                .OrderBy(m => m.StudentSection.Student.StudentName)
                .ThenBy(m => m.StudentSection.Student.OtherName)
                .ThenBy(m => m.StudentSection.Section.SectionName);
        }

        public static IOrderedQueryable<SectionInfo> Ordered(this IQueryable<SectionInfo> query)
        {
            return query
                .OrderByDescending(m => m.AcademicYear.AcademicYearName)
                .ThenBy(m => m.Course.CourseName)
                .ThenBy(m => m.Class.ClassName)
                .ThenBy(m => m.Section.SectionName);
        }

        public static IOrderedQueryable<StudentSectionInfo> Ordered(this IQueryable<StudentSectionInfo> query)
        {
            return query
                .OrderBy(m => m.Student.StudentName)
                .ThenBy(m => m.Student.OtherName)
                .ThenBy(m => m.School.SchoolName)
                .ThenByDescending(m => m.AcademicYear.AcademicYearName)
                .ThenBy(m => m.Course.CourseName)
                .ThenBy(m => m.Class.ClassName)
                .ThenBy(m => m.Section.SectionName);
        }

        public static IOrderedQueryable<User> Ordered(this IQueryable<User> query)
        {
            return query
                .OrderBy(m => m.UserName);
        }

        public static IQueryable<AcademicYear> Include(this IQueryable<AcademicYear> query)
        {
            return query
                .Include(m => m.School);
        }

        public static IQueryable<Course> Include(this IQueryable<Course> query)
        {
            return query
                .Include(m => m.AcademicYear.School);
        }

        public static IQueryable<Class> Include(this IQueryable<Class> query)
        {
            return query
                .Include(m => m.Course.AcademicYear.School);
        }

        public static IQueryable<Section> Include(this IQueryable<Section> query)
        {
            return query
                .Include(m => m.Class.Course.AcademicYear.School)
                .Include(m => m.Currency);
        }

        public static IQueryable<Receipt> Include(this IQueryable<Receipt> query)
        {
            return query
                .Include(m => m.ReceiptDetails);
        }

        public static IQueryable<School> Include(this IQueryable<School> query)
        {
            return query
                .Include(m => m.Address)
                .Include(m => m.SchoolEmails)
                .Include(m => m.SchoolPhones);
        }

        public static IQueryable<StudentDiscount> Include(this IQueryable<StudentDiscount> query)
        {
            return query
                .Include(m => m.StudentSection.Student)
                .Include(m => m.StudentSection.Section);
        }

        public static IQueryable<StudentFerryFee> Include(this IQueryable<StudentFerryFee> query)
        {
            return query
                .Include(m => m.StudentSection.Student)
                .Include(m => m.StudentSection.Section);
        }

        public static IQueryable<StudentSection> Include(this IQueryable<StudentSection> query, 
            bool currency = false, bool discount = false, bool ferry = false)
        {
            query = query
                .Include(m => m.Student)
                .Include(m => m.Section);

            if (currency)
                query = query.Include(m => m.Section.Currency);
            if (discount)
                query = query.Include(m => m.StudentDiscount);
            if (ferry)
                query = query.Include(m => m.StudentFerryFee);

            return query;
        }

        public static IQueryable<User> Include(this IQueryable<User> query)
        {
            return query
                .Include(m => m.UserMembership)
                .Include(m => m.UserPermissions);
        }

        //    0"Section Name", 1"Section Name [ OD ]", 2"Section Name [ ST ~ ET ]", 3"Section Name [ OD, ST ~ ET ]",
        //    4"Section Name [ Room ]", 5"Section Name [ OD, Room ]", 6"Section Name [ ST ~ ET, Room ]",
        //    7"Section Name [ OD, ST ~ ET, Room ]", 8"Section Name [ CMD ~ CPD ]",
        //    9"Section Name [ Room, CMD ~ CPD ]", 10"Section Name [ OD, ST ~ ET, CMD ~ CPD ]",
        //    11"Section Name [ OD, ST ~ ET, Room, CMD ~ CPD ]");
        public static Func<Section, string> GetSectionName(int index)
        {
            if (index == 0)
                return (m) => m.SectionName;
            if (index == 1)
                return (m) => m.SectionNameWithOpenDay;
            if (index == 2)
                return (m) => m.SectionNameWithStartEndTime;
            if (index == 3)
                return (m) => m.SectionNameWithOpenDayStartEndTime;
            if (index == 4)
                return (m) =>
                {
                    if (string.IsNullOrEmpty(m.Room))
                        return m.SectionName;

                    return string.Format("{0} [ {1} ]", m.SectionName, m.Room);
                };
            if (index == 5)
                return (m) =>
                {
                    var value = ", ".Combine(m.OpenDay, m.Room);

                    if (string.IsNullOrEmpty(value))
                        return m.SectionName;

                    return string.Format("{0} [ {1} ]", m.SectionName, value);
                };
            if (index == 6)
                return (m) =>
                {
                    var value = ", ".Combine(m.StartEndTime, m.Room);

                    if (string.IsNullOrEmpty(value))
                        return m.SectionName;

                    return string.Format("{0} [ {1} ]", m.SectionName, value);
                };
            if (index == 7)
                return (m) =>
                {
                    var value = ", ".Combine(m.OpenDay, m.StartEndTime, m.Room);

                    if (string.IsNullOrEmpty(value))
                        return m.SectionName;

                    return string.Format("{0} [ {1} ]", m.SectionName, value);
                };
            if (index == 8)
                return (m) =>
                {
                    var value = m.CommencingCompletionDate;

                    if (string.IsNullOrEmpty(value))
                        return m.SectionName;

                    return string.Format("{0} [ {1} ]", m.SectionName, value);
                };
            if (index == 9)
                return (m) =>
                {
                    var value = ", ".Combine(m.Room, m.CommencingCompletionDate);

                    if (string.IsNullOrEmpty(value))
                        return m.SectionName;

                    return string.Format("{0} [ {1} ]", m.SectionName, value);
                };
            if (index == 10)
                return (m) =>
                {
                    var value = ", ".Combine(m.OpenDay, m.StartEndTime, m.CommencingCompletionDate);

                    if (string.IsNullOrEmpty(value))
                        return m.SectionName;

                    return string.Format("{0} [ {1} ]", m.SectionName, value);
                };
            if (index == 11)
                return (m) =>
                {
                    var value = ", ".Combine(m.OpenDay, m.StartEndTime, m.Room, m.CommencingCompletionDate);

                    if (string.IsNullOrEmpty(value))
                        return m.SectionName;

                    return string.Format("{0} [ {1} ]", m.SectionName, value);
                };

            return (m) => m.SectionName;
        }

        public static Func<Student, string> GetStudentName(int index)
        {
            if (index == 0)
                return (m) => m.StudentName;
            if (index == 1)
                return (m) => m.OtherName;
            if (index == 2)
                return (m) => m.StudentFullName;
            if (index == 3)
                return (m) => " ".Combine(m.Title, m.StudentName);
            if (index == 4)
                return (m) => " ".Combine(m.Title, m.OtherName);
            if (index == 5)
                return (m) => m.TitleStudentFullName;

            return (m) => m.StudentName;
        }

        public static Func<Address, string> GetAddress(int index)
        {
            if (index == 0)
                return (m) => m.Line1;
            if (index == 1)
                return (m) => m.Line1City;
            if (index == 2)
                return (m) => m.Line1Township;
            if (index == 3)
                return (m) => m.Line1CityTownship;
            if (index == 4)
                return (m) => m.LongAddress;

            return (m) => m.Line1;
        }

        public static Func<Guardian, string> GetGuardianName(int index)
        {
            if (index == 0)
                return (m) => m.GuardianName;
            if (index == 1)
                return (m) => m.TitleGuardianName;

            return (m) => m.GuardianName;
        }
    }
}
