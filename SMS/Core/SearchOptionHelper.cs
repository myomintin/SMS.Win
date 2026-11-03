using CodeflexSystems.Library.Expressions;
using CodeflexSystems.Library.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    internal static class SearchOptionHelper
    {
        #region Match Types & Options

        public static IEnumerable<ExactMatchType> GetExactMatchTypes()
        {
            return ReflectionHelper.ExtractEnumValues<ExactMatchType>();
        }

        public static IEnumerable<ValueMatchType> GetValueMatchTypes()
        {
            return ReflectionHelper.ExtractEnumValues<ValueMatchType>();
        }

        public static IEnumerable<StringMatchType> GetStringMatchTypes()
        {
            return ReflectionHelper.ExtractEnumValues<StringMatchType>();
        }

        public static IEnumerable<AddressSearchOption> GetAddressSearchOptions()
        {
            return ReflectionHelper.ExtractEnumValues<AddressSearchOption>();
        }

        public static IEnumerable<GuardianSearchOption> GetGuardianSearchOptions()
        {
            return ReflectionHelper.ExtractEnumValues<GuardianSearchOption>();
        }

        public static IEnumerable<ReceiptSearchOption> GetReceiptSearchOptions()
        {
            return ReflectionHelper.ExtractEnumValues<ReceiptSearchOption>();
        }

        public static IEnumerable<SectionSearchOption> GetSectionSearchOptions()
        {
            return ReflectionHelper.ExtractEnumValues<SectionSearchOption>();
        }

        public static IEnumerable<StudentDiscountSearchOption> GetStudentDiscountSearchOptions()
        {
            return ReflectionHelper.ExtractEnumValues<StudentDiscountSearchOption>();
        }

        public static IEnumerable<StudentFerryFeeSearchOption> GetStudentFerryFeeSearchOptions()
        {
            return ReflectionHelper.ExtractEnumValues<StudentFerryFeeSearchOption>();
        }

        public static IEnumerable<StudentSearchOption> GetStudentSearchOptions()
        {
            return ReflectionHelper.ExtractEnumValues<StudentSearchOption>();
        }

        public static IEnumerable<UserSearchOption> GetUserSearchOptions()
        {
            return ReflectionHelper.ExtractEnumValues<UserSearchOption>();
        }

        public static Tuple<ExactMatchType, string> GetMatchTypeInfo(ExactMatchType mt)
        {
            if (mt == ExactMatchType.Equal)
                return new Tuple<ExactMatchType, string>(ExactMatchType.Equal, "Equal");
            else if (mt == ExactMatchType.NotEqual)
                return new Tuple<ExactMatchType, string>(ExactMatchType.NotEqual, "Not Equal");
            else
                return new Tuple<ExactMatchType, string>(ExactMatchType.NoValue, "No Value");
        }

        public static Tuple<ValueMatchType, string> GetMatchTypeInfo(ValueMatchType mt)
        {
            if (mt == ValueMatchType.Equal)
                return new Tuple<ValueMatchType, string>(ValueMatchType.Equal, "Equal");
            else if (mt == ValueMatchType.NotEqual)
                return new Tuple<ValueMatchType, string>(ValueMatchType.NotEqual, "Not Equal");
            if (mt == ValueMatchType.GreaterThan)
                return new Tuple<ValueMatchType, string>(ValueMatchType.GreaterThan, "Greater Than");
            else if (mt == ValueMatchType.GreaterThanOrEqual)
                return new Tuple<ValueMatchType, string>(ValueMatchType.GreaterThanOrEqual, "Greater Than Or Equal");
            if (mt == ValueMatchType.LessThan)
                return new Tuple<ValueMatchType, string>(ValueMatchType.LessThan, "Less Than");
            else if (mt == ValueMatchType.LessThanOrEqual)
                return new Tuple<ValueMatchType, string>(ValueMatchType.LessThanOrEqual, "Less Than Or Equal");
            else
                return new Tuple<ValueMatchType, string>(ValueMatchType.NoValue, "No Value");
        }

        public static Tuple<StringMatchType, string> GetMatchTypeInfo(StringMatchType mt)
        {
            if (mt == StringMatchType.Equal)
                return new Tuple<StringMatchType, string>(StringMatchType.Equal, "Equal");
            else if (mt == StringMatchType.NotEqual)
                return new Tuple<StringMatchType, string>(StringMatchType.NotEqual, "Not Equal");
            else if (mt == StringMatchType.Contains)
                return new Tuple<StringMatchType, string>(StringMatchType.Contains, "Contains");
            else if (mt == StringMatchType.StartsWith)
                return new Tuple<StringMatchType, string>(StringMatchType.StartsWith, "Starts With");
            else if (mt == StringMatchType.EndsWith)
                return new Tuple<StringMatchType, string>(StringMatchType.EndsWith, "Ends With");
            else
                return new Tuple<StringMatchType, string>(StringMatchType.NoValue, "No Value");
        }

        #endregion

        public static SearchOptionInfo<AddressSearchOption> GetSearchInfo(AddressSearchOption option)
        {
            var info = new SearchOptionInfo<AddressSearchOption>() { Nullable = true };

            if (option == AddressSearchOption.Line1)
            {
                info.Name = "Line1";
                info.Information = "Search the addresses by Line1.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
                info.Nullable = false;
            }
            else if (option == AddressSearchOption.Line2)
            {
                info.Name = "Line2";
                info.Information = "Search the addresses by Line2.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == AddressSearchOption.ZipCode)
            {
                info.Name = "Zip Code";
                info.Information = "Search the addresses by Zip Code.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == AddressSearchOption.City)
            {
                info.Name = "City";
                info.Information = "Search the addresses by City.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == AddressSearchOption.Township)
            {
                info.Name = "Township";
                info.Information = "Search the addresses by Township.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == AddressSearchOption.State)
            {
                info.Name = "State";
                info.Information = "Search the addresses by State.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == AddressSearchOption.Country)
            {
                info.Name = "Country";
                info.Information = "Search the addresses by Country.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Object;
                info.Nullable = false;
            }
            else if (option == AddressSearchOption.CreatedDate)
            {
                info.Name = "Created Date";
                info.Information = "Search the addresses by Created Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == AddressSearchOption.CreatedBy)
            {
                info.Name = "Created By";
                info.Information = "Search the addresses that are created by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == AddressSearchOption.ModifiedDate)
            {
                info.Name = "Modified Date";
                info.Information = "Search the addresses by Modified Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == AddressSearchOption.ModifiedBy)
            {
                info.Name = "Modified By";
                info.Information = "Search the addresses that are modified by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else
            {
                info.Name = "No Usage";
                info.Information = "Search the addresses that do not used.";
                info.Option = option;
                info.SearchValueType = SearchValueType.NoneType;
            }

            return info;
        }

        public static SearchOptionInfo<GuardianSearchOption> GetSearchInfo(GuardianSearchOption option)
        {
            var info = new SearchOptionInfo<GuardianSearchOption>() { Nullable = true };

            if (option == GuardianSearchOption.Title)
            {
                info.Name = "Title";
                info.Information = "Search the guardians by Title.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == GuardianSearchOption.GuardianName)
            {
                info.Name = "Guardian Name";
                info.Information = "Search the guardians by Guardian Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
                info.Nullable = false;
            }
            else if (option == GuardianSearchOption.Nationality)
            {
                info.Name = "Nationality";
                info.Information = "Search the guardians by Nationality.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == GuardianSearchOption.NIRC)
            {
                info.Name = "NIRC";
                info.Information = "Search the guardians by NIRC.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == GuardianSearchOption.Education)
            {
                info.Name = "Education";
                info.Information = "Search the guardians by Education.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == GuardianSearchOption.Employment)
            {
                info.Name = "Employment";
                info.Information = "Search the guardians by Employment.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == GuardianSearchOption.Designation)
            {
                info.Name = "Designation";
                info.Information = "Search the guardians by Designation.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == GuardianSearchOption.Passport)
            {
                info.Name = "Passport";
                info.Information = "Search the guardians by Passport.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == GuardianSearchOption.Visa)
            {
                info.Name = "Visa";
                info.Information = "Search the guardians by Visa.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == GuardianSearchOption.BirthDate)
            {
                info.Name = "Birth Date";
                info.Information = "Search the guardians by Birth Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
            }
            else if (option == GuardianSearchOption.BirthCountry)
            {
                info.Name = "Birth Country";
                info.Information = "Search the guardians by Birth Country.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Object;
            }
            else if (option == GuardianSearchOption.Religion)
            {
                info.Name = "Religion";
                info.Information = "Search the guardians by Religion.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Object;
            }
            else if (option == GuardianSearchOption.CreatedDate)
            {
                info.Name = "Created Date";
                info.Information = "Search the guardians by Created Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == GuardianSearchOption.CreatedBy)
            {
                info.Name = "Created By";
                info.Information = "Search the guardians that are created by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == GuardianSearchOption.ModifiedDate)
            {
                info.Name = "Modified Date";
                info.Information = "Search the guardians by Modified Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == GuardianSearchOption.ModifiedBy)
            {
                info.Name = "Modified By";
                info.Information = "Search the guardians that are modified by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else
            {
                info.Name = "No Usage";
                info.Information = "Search the guardians who do not have any children.";
                info.Option = option;
                info.SearchValueType = SearchValueType.NoneType;
            }

            return info;
        }

        public static SearchOptionInfo<ReceiptSearchOption> GetSearchInfo(ReceiptSearchOption option)
        {
            var info = new SearchOptionInfo<ReceiptSearchOption>() { Nullable = true };

            if (option == ReceiptSearchOption.ReceiptDate)
            {
                info.Name = "Receipt Date";
                info.Information = "Search the receipts by Receipt Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == ReceiptSearchOption.ReceiptCode)
            {
                info.Name = "Receipt Code";
                info.Information = "Search the receipts by Receipt Code.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == ReceiptSearchOption.ReceiptNo)
            {
                info.Name = "Receipt No";
                info.Information = "Search the receipts by Receipt No.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Integral;
                info.Nullable = false;
            }
            else if (option == ReceiptSearchOption.TotalAmount)
            {
                info.Name = "Total Amount";
                info.Information = "Search the receipts by Total Amount.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Decimal;
                info.Nullable = false;
            }
            else if (option == ReceiptSearchOption.TaxAmount)
            {
                info.Name = "Tax Amount";
                info.Information = "Search the receipts by Tax Amount.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Decimal;
            }
            else if (option == ReceiptSearchOption.TaxPercentage)
            {
                info.Name = "Tax Percentage";
                info.Information = "Search the receipts by Tax Percentage.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Percentage;
                info.Nullable = false;
            }
            else if (option == ReceiptSearchOption.NetAmount)
            {
                info.Name = "Net Amount";
                info.Information = "Search the receipts by Net Amount.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Decimal;
                info.Nullable = false;
            }
            else if (option == ReceiptSearchOption.CreatedDate)
            {
                info.Name = "Created Date";
                info.Information = "Search the receipts by Created Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == ReceiptSearchOption.CreatedBy)
            {
                info.Name = "Created By";
                info.Information = "Search the receipts that are created by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == ReceiptSearchOption.ModifiedDate)
            {
                info.Name = "Modified Date";
                info.Information = "Search the receipts by Modified Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == ReceiptSearchOption.ModifiedBy)
            {
                info.Name = "Modified By";
                info.Information = "Search the receipts that are modified by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == ReceiptSearchOption.DetailAmount)
            {
                info.Name = "Detail's Amount";
                info.Information = "Search the receipts by Receipt Detail's Amount.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Decimal;
                info.Nullable = false;
            }
            else if (option == ReceiptSearchOption.DetailDiscountAmount)
            {
                info.Name = "Detail's Discount Amount";
                info.Information = "Search the receipts by Receipt Detail's Discount Amount.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Decimal;
            }
            else if (option == ReceiptSearchOption.DetailNetAmount)
            {
                info.Name = "Detail's Net Amount";
                info.Information = "Search the receipts by Receipt Detail's Net Amount.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Decimal;
                info.Nullable = false;
            }
            else if (option == ReceiptSearchOption.Student)
            {
                info.Name = "Student";
                info.Information = "Search the receipts by Student.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Object;
                info.Nullable = false;
            }
            else
            {
                info.Name = "No Usage";
                info.Information = "Search the receipt who do not have any receipt details.";
                info.Option = option;
                info.SearchValueType = SearchValueType.NoneType;
            }

            return info;
        }

        public static SearchOptionInfo<SectionSearchOption> GetSearchInfo(SectionSearchOption option)
        {
            var info = new SearchOptionInfo<SectionSearchOption>() { Nullable = true };

            if (option == SectionSearchOption.SectionName)
            {
                info.Name = "Section Name";
                info.Information = "Search the sections by Section Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
                info.Nullable = false;
            }
            else if (option == SectionSearchOption.Room)
            {
                info.Name = "Room";
                info.Information = "Search the sections by Room.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == SectionSearchOption.OpenDay)
            {
                info.Name = "Open Day";
                info.Information = "Search the sections by Open Day.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == SectionSearchOption.StartTime)
            {
                info.Name = "Start Time";
                info.Information = "Search the sections by Start Time.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Time;
            }
            else if (option == SectionSearchOption.EndTime)
            {
                info.Name = "End Time";
                info.Information = "Search the sections by End Time.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Time;
            }
            else if (option == SectionSearchOption.CommencingDate)
            {
                info.Name = "Commencing Date";
                info.Information = "Search the sections by Commencing Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
            }
            else if (option == SectionSearchOption.CompletionDate)
            {
                info.Name = "Completion Date";
                info.Information = "Search the sections by Completion Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
            }
            else if (option == SectionSearchOption.Fee)
            {
                info.Name = "Section Fee";
                info.Information = "Search the sections by Section Fee.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Decimal;
            }
            else if (option == SectionSearchOption.Monthly)
            {
                info.Name = "Monthly";
                info.Information = "Search the sections by Monthly.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Boolean;
                info.Nullable = false;
            }
            else if (option == SectionSearchOption.CreatedDate)
            {
                info.Name = "Created Date";
                info.Information = "Search the sections by Created Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == SectionSearchOption.CreatedBy)
            {
                info.Name = "Created By";
                info.Information = "Search the sections that are created by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == SectionSearchOption.ModifiedDate)
            {
                info.Name = "Modified Date";
                info.Information = "Search the sections by Modified Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == SectionSearchOption.ModifiedBy)
            {
                info.Name = "Modified By";
                info.Information = "Search the sections that are modified by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else
            {
                info.Name = "No Usage";
                info.Information = "Search the sections that do not used.";
                info.Option = option;
                info.SearchValueType = SearchValueType.NoneType;
            }

            return info;
        }

        public static SearchOptionInfo<StudentDiscountSearchOption> GetSearchInfo(StudentDiscountSearchOption option)
        {
            var info = new SearchOptionInfo<StudentDiscountSearchOption>() { Nullable = true };

            if (option == StudentDiscountSearchOption.DiscountAmount)
            {
                info.Name = "Discount Amount";
                info.Information = "Search the discounts by Discount Amount.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Decimal;
                info.Nullable = false;
            }
            else if (option == StudentDiscountSearchOption.DiscountPercentage)
            {
                info.Name = "Discount Percentage";
                info.Information = "Search the discounts by Discount Percentage.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Percentage;
                info.Nullable = false;
            }
            else if (option == StudentDiscountSearchOption.CreatedDate)
            {
                info.Name = "Created Date";
                info.Information = "Search the discounts by Created Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == StudentDiscountSearchOption.CreatedBy)
            {
                info.Name = "Created By";
                info.Information = "Search the discounts that are created by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == StudentDiscountSearchOption.ModifiedDate)
            {
                info.Name = "Modified Date";
                info.Information = "Search the discounts by Modified Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == StudentDiscountSearchOption.ModifiedBy)
            {
                info.Name = "Modified By";
                info.Information = "Search the discounts that are modified by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == StudentDiscountSearchOption.Student)
            {
                info.Name = "Student";
                info.Information = "Search the discounts by Student.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Object;
                info.Nullable = false;
            }
            else
            {
                info.Name = "No Usage";
                info.Information = "Search the discounts that do not used.";
                info.Option = option;
                info.SearchValueType = SearchValueType.NoneType;
            }

            return info;
        }

        public static SearchOptionInfo<StudentFerryFeeSearchOption> GetSearchInfo(StudentFerryFeeSearchOption option)
        {
            var info = new SearchOptionInfo<StudentFerryFeeSearchOption>() { Nullable = true };

            if (option == StudentFerryFeeSearchOption.Fee)
            {
                info.Name = "Fee Amount";
                info.Information = "Search the ferry fees by Fee Amount.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Decimal;
                info.Nullable = false;
            }
            else if (option == StudentFerryFeeSearchOption.Monthly)
            {
                info.Name = "Monthly";
                info.Information = "Search the ferry fees by Monthly.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Boolean;
                info.Nullable = false;
            }
            else if (option == StudentFerryFeeSearchOption.CreatedDate)
            {
                info.Name = "Created Date";
                info.Information = "Search the ferry fees by Created Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == StudentFerryFeeSearchOption.CreatedBy)
            {
                info.Name = "Created By";
                info.Information = "Search the ferry fees that are created by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == StudentFerryFeeSearchOption.ModifiedDate)
            {
                info.Name = "Modified Date";
                info.Information = "Search the ferry fees by Modified Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == StudentFerryFeeSearchOption.ModifiedBy)
            {
                info.Name = "Modified By";
                info.Information = "Search the ferry fees that are modified by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == StudentFerryFeeSearchOption.Student)
            {
                info.Name = "Student";
                info.Information = "Search the ferry fees by Student.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Object;
                info.Nullable = false;
            }
            else
            {
                info.Name = "No Usage";
                info.Information = "Search the ferry fees that do not used.";
                info.Option = option;
                info.SearchValueType = SearchValueType.NoneType;
            }

            return info;
        }

        public static SearchOptionInfo<StudentSearchOption> GetSearchInfo(StudentSearchOption option)
        {
            var info = new SearchOptionInfo<StudentSearchOption>() { Nullable = true };

            if (option == StudentSearchOption.Id)
            {
                info.Name = "Id";
                info.Information = "Search the students by Student Id.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == StudentSearchOption.Title)
            {
                info.Name = "Title";
                info.Information = "Search the students by Title.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == StudentSearchOption.StudentName)
            {
                info.Name = "Student Name";
                info.Information = "Search the students by Student Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
                info.Nullable = false;
            }
            else if (option == StudentSearchOption.OtherName)
            {
                info.Name = "Other Name";
                info.Information = "Search the students by Other Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == StudentSearchOption.StudentFullName)
            {
                info.Name = "Student Full Name";
                info.Information = "Search the students by Student Name and Other Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
                info.Nullable = false;
            }
            else if (option == StudentSearchOption.Nationality)
            {
                info.Name = "Nationality";
                info.Information = "Search the students by Nationality.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == StudentSearchOption.NIRC)
            {
                info.Name = "NIRC";
                info.Information = "Search the students by NIRC.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == StudentSearchOption.Passport)
            {
                info.Name = "Passport";
                info.Information = "Search the students by Passport.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == StudentSearchOption.Visa)
            {
                info.Name = "Visa";
                info.Information = "Search the students by Visa.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == StudentSearchOption.BirthDate)
            {
                info.Name = "Birth Date";
                info.Information = "Search the students by Birth Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
            }
            else if (option == StudentSearchOption.BirthCountry)
            {
                info.Name = "Birth Country";
                info.Information = "Search the students by Birth Country.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Object;
            }
            else if (option == StudentSearchOption.Religion)
            {
                info.Name = "Religion";
                info.Information = "Search the students by Religion.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Object;
            }
            else if (option == StudentSearchOption.CreatedDate)
            {
                info.Name = "Created Date";
                info.Information = "Search the students by Created Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == StudentSearchOption.CreatedBy)
            {
                info.Name = "Created By";
                info.Information = "Search the students that are created by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == StudentSearchOption.ModifiedDate)
            {
                info.Name = "Modified Date";
                info.Information = "Search the students by Modified Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == StudentSearchOption.ModifiedBy)
            {
                info.Name = "Modified By";
                info.Information = "Search the students that are modified by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == StudentSearchOption.RegisteredDate)
            {
                info.Name = "Registered Date";
                info.Information = "Search the students by Registered Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
            }
            else if (option == StudentSearchOption.ResignedDate)
            {
                info.Name = "Resigned Date";
                info.Information = "Search the students by Resigned Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
            }
            else if (option == StudentSearchOption.Active)
            {
                info.Name = "Active";
                info.Information = "Search the students by Active Education.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Boolean;
                info.Nullable = false;
            }
            else
            {
                info.Name = "No Usage";
                info.Information = "Search the students who do not have any sections.";
                info.Option = option;
                info.SearchValueType = SearchValueType.NoneType;
            }

            return info;
        }

        public static SearchOptionInfo<UserSearchOption> GetSearchInfo(UserSearchOption option)
        {
            var info = new SearchOptionInfo<UserSearchOption>() { Nullable = true };

            if (option == UserSearchOption.UserName)
            {
                info.Name = "User Name";
                info.Information = "Search the users by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
                info.Nullable = false;
            }
            else if (option == UserSearchOption.FullName)
            {
                info.Name = "Full Name";
                info.Information = "Search the users by Full Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == UserSearchOption.CreatedDate)
            {
                info.Name = "Created Date";
                info.Information = "Search the users by Created Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == UserSearchOption.CreatedBy)
            {
                info.Name = "Created By";
                info.Information = "Search the users that are created by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == UserSearchOption.ModifiedDate)
            {
                info.Name = "Modified Date";
                info.Information = "Search the users by Modified Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
                info.Nullable = false;
            }
            else if (option == UserSearchOption.ModifiedBy)
            {
                info.Name = "Modified By";
                info.Information = "Search the users that are modified by User Name.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Text;
            }
            else if (option == UserSearchOption.Disabled)
            {
                info.Name = "Disabled";
                info.Information = "Search the users by Disabled.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Boolean;
                info.Nullable = false;
            }
            else if (option == UserSearchOption.DisabledDate)
            {
                info.Name = "Disabled Date";
                info.Information = "Search the users by Disabled Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
            }
            else if (option == UserSearchOption.LogonDate)
            {
                info.Name = "Logon Date";
                info.Information = "Search the users by Logon Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
            }
            else if (option == UserSearchOption.PasswordModifiedDate)
            {
                info.Name = "Password Modified Date";
                info.Information = "Search the users by Password Modified Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
            }
            else if (option == UserSearchOption.PasswordFailures)
            {
                info.Name = "Password Failures";
                info.Information = "Search the users by Password Failure times.";
                info.Option = option;
                info.SearchValueType = SearchValueType.Integral;
                info.Nullable = false;
            }
            else if (option == UserSearchOption.PasswordFailureDate)
            {
                info.Name = "Password Failure Date";
                info.Information = "Search the users by Password Failure Date.";
                info.Option = option;
                info.SearchValueType = SearchValueType.DateTime;
            }
            else
            {
                info.Name = "No Usage";
                info.Information = "Search the users who do not have any usage.";
                info.Option = option;
                info.SearchValueType = SearchValueType.NoneType;
            }

            return info;
        }
    }
}
