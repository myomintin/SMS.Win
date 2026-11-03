using CodeflexSystems.Library.Extensions;
using CodeflexSystems.Library.Reflection;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using CodeflexSystems.SMS.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Core
{
    internal static class ReportHelper
    {
        private static string sectionDescription = "OD = Open Day, ST = Start Time, ET = End Time, CMD = Commencing Date, CPD = Completion Date.";

        public static IEnumerable<ReportType> GetReportTypes()
        {
            return ReflectionHelper.ExtractEnumValues<ReportType>();
        }

        public static IEnumerable<ReportType> GetSchoolReportTypes()
        {
            var a = (ReportType)20000;

            return GetReportTypes().Where(m => m < a);
        }

        public static IEnumerable<ReportType> GetTypeReportTypes()
        {
            var t = (ReportType)11000;
            var a = (ReportType)20000;

            return GetReportTypes().Where(m => m >= t && m < a);
        }

        public static IEnumerable<ReportType> GetAccountReportTypes()
        {
            var a = (ReportType)20000;

            return GetReportTypes().Where(m => m >= a);
        }

        public static ReportInfo GetReportInfo(ReportType rt)
        {
            var ri = new ReportInfo
            {
                ReportType = rt,
                DisplayLevel = ReportLevel.None,
                ShowActive = false,
                ShowSectionNameFormat = false,
                ShowStudentNameFormat = false,
                ShowAddressFormat = false
            };

            if (rt == ReportType.StudentList)
            {
                ri.Name = "Student List";
                ri.Information = Environment.NewLine.Combine(
                    "The student list in the checked sections. Id, Student Name.",
                    sectionDescription);
                ri.DisplayLevel = ReportLevel.Section;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.StudentListDetails)
            {
                ri.Name = "Student List Details";
                ri.Information = Environment.NewLine.Combine(
                    "The student list in the checked sections. Id, Student Name, Gender, Registration Date, Resigned Date, Active.",
                    sectionDescription);
                ri.DisplayLevel = ReportLevel.Section;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.StudentListAge)
            {
                ri.Name = "Student List Age";
                ri.Information = Environment.NewLine.Combine(
                    "The student list in the checked sections. Id, Student Name, Gender, Birth Date, Age, Active.",
                    sectionDescription);
                ri.DisplayLevel = ReportLevel.Section;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.StudentListInfo)
            {
                ri.Name = "Student List Info";
                ri.Information = Environment.NewLine.Combine(
                    "The student information in the checked students. Id, Student Name, Addresses, Emails, Phones, Guardians.",
                    sectionDescription);
                ri.DisplayLevel = ReportLevel.StudentSection;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
                ri.ShowAddressFormat = true;
            }
            else if (rt == ReportType.StudentListInRoom)
            {
                ri.Name = "Student List In Room";
                ri.Information = Environment.NewLine.Combine(
                    "The active student list in the checked sections. Id, Student Name, Gender.",
                    sectionDescription);
                ri.DisplayLevel = ReportLevel.Section;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.StudentListAddress)
            {
                ri.Name = "Student List Address";
                ri.Information = Environment.NewLine.Combine(
                    "The student list in the checked sections. Id, Student Name, Addresses.",
                    sectionDescription, "Style : Portrait.");
                ri.DisplayLevel = ReportLevel.Section;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.StudentListAddress2)
            {
                ri.Name = "Student List Address style #2";
                ri.Information = Environment.NewLine.Combine(
                    "The student list in the checked sections. Id, Student Name, Addresses.",
                    sectionDescription, "Style : Landscape.");
                ri.DisplayLevel = ReportLevel.Section;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.StudentListEmail)
            {
                ri.Name = "Student List Email";
                ri.Information = Environment.NewLine.Combine(
                    "The student list in the checked sections. Id, Student Name, Emails.",
                    sectionDescription, "Style : Portrait.");
                ri.DisplayLevel = ReportLevel.Section;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.StudentListEmail2)
            {
                ri.Name = "Student List Email style #2";
                ri.Information = Environment.NewLine.Combine(
                    "The student list in the checked sections. Id, Student Name, Emails.",
                    sectionDescription, "Style : Landscape.");
                ri.DisplayLevel = ReportLevel.Section;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.StudentListPhone)
            {
                ri.Name = "Student List Phone";
                ri.Information = Environment.NewLine.Combine(
                    "The student list in the checked sections. Id, Student Name, Phones.",
                    sectionDescription, "Style : Portrait.");
                ri.DisplayLevel = ReportLevel.Section;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.StudentListPhone2)
            {
                ri.Name = "Student List Phone style #2";
                ri.Information = Environment.NewLine.Combine(
                    "The student list in the checked sections. Id, Student Name, Phones.",
                    sectionDescription, "Style : Landscape.");
                ri.DisplayLevel = ReportLevel.Section;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.StudentListGuardian)
            {
                ri.Name = "Student List Guardian";
                ri.Information = Environment.NewLine.Combine(
                    "The student list in the checked sections. Id, Student Name, Guardians.",
                    sectionDescription, "Style : Portrait.");
                ri.DisplayLevel = ReportLevel.Section;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.StudentListGuardian2)
            {
                ri.Name = "Student List Guardian style #2";
                ri.Information = Environment.NewLine.Combine(
                    "The student list in the checked sections. Id, Student Name, Guardians.",
                    sectionDescription, "Style : Landscape.");
                ri.DisplayLevel = ReportLevel.Section;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.StudentCard)
            {
                ri.Name = "Student Card";
                ri.Information = "The studend card in the checked students.";
                ri.DisplayLevel = ReportLevel.StudentSection;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
                ri.ShowAddressFormat = true;
            }
            else if (rt == ReportType.StudentDiscount)
            {
                ri.Name = "Student Discount List";
                ri.Information = Environment.NewLine.Combine(
                    "The student discount list in the checked courses. Id, Student Name, Section, Discount Percentage, Discount Amount.",
                    sectionDescription);
                ri.DisplayLevel = ReportLevel.Course;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.StudentFerryFee)
            {
                ri.Name = "Student Ferry Fee List";
                ri.Information = Environment.NewLine.Combine(
                    "The student ferry fee list in the checked courses. Id, Student Name, Section, Ferry Fee, Monthly.",
                    sectionDescription);
                ri.DisplayLevel = ReportLevel.Course;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.Reminder)
            {
                ri.Name = "Reminder List";
                ri.Information = Environment.NewLine.Combine(
                    "The reminder for the students. Payment Month, Id, Student Name, Section, School Fee, Ferry Fee, Discount, Total, Due Date.",
                    sectionDescription);
                ri.DisplayLevel = ReportLevel.StudentSection;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
                ri.ShowAddressFormat = true;
            }
            else if (rt == ReportType.Reminder2)
            {
                ri.Name = "Reminder List style #2";
                ri.Information = Environment.NewLine.Combine(
                    "The reminder with column for the students. Payment Month, Id, Student Name, Section, School Fee, Ferry Fee, Discount, Total, Due Date.",
                    sectionDescription, "Style : column.");
                ri.DisplayLevel = ReportLevel.StudentSection;
                ri.ShowActive = true;
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
                ri.ShowAddressFormat = true;
            }
            else if (rt == ReportType.DailyIncome)
            {
                ri.Name = "Daily Income";
                ri.Information = "The daily income of the schools. Code No., Total Amount, Tax, Tax Percentage, Net Amount of Receipt.";
            }
            else if (rt == ReportType.DailyIncomeDetails)
            {
                ri.Name = "Daily Income Details";
                ri.Information = "The daily income details information of the schools. Code No., Total Amount, Tax, Tax Percentage, Net Amount of Receipt. Id, Student Name, Section, Fee Type, Month, Quantity, Amount, Total Amount, Discount, Net Amount of Receipt Details.";
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.DailyIncomeDetails2)
            {
                ri.Name = "Daily Income Details style #2";
                ri.Information = "The daily income details information of the schools. Code No., Total Amount, Tax, Tax Percentage, Net Amount of Receipt. Id, Student Name, Section, Fee Type, Month, Quantity, Amount, Total Amount, Discount, Net Amount of Receipt Details.";
                ri.ShowSectionNameFormat = true;
                ri.ShowStudentNameFormat = true;
            }
            else if (rt == ReportType.MonthlyIncome)
            {
                ri.Name = "Monthly Income";
                ri.Information = "The monthly income of the schools. Receipt Date, Total Amount, Tax Amount, Net Amount.";
            }
            else if (rt == ReportType.YearlyIncome)
            {
                ri.Name = "Yearly Income";
                ri.Information = "The yearly income of the schools. Receipt Date, Total Amount, Tax Amount, Net Amount.";
            }
            else if (rt == ReportType.School)
            {
                ri.Name = "School List";
                ri.Information = "The schools that configured in the system.";
                ri.DisplayLevel = ReportLevel.School;
            }
            else if (rt == ReportType.AcademicYear)
            {
                ri.Name = "Academic Year List";
                ri.Information = "The academic years of the checked school.";
                ri.DisplayLevel = ReportLevel.AcademicYear;
            }
            else if (rt == ReportType.Course)
            {
                ri.Name = "Course List";
                ri.Information = "The courses of the checked academic year and school.";
                ri.DisplayLevel = ReportLevel.Course;
            }
            else if (rt == ReportType.Class)
            {
                ri.Name = "Class List";
                ri.Information = "The classes of the checked academic year and school.";
                ri.DisplayLevel = ReportLevel.Class;
            }
            else if (rt == ReportType.Section)
            {
                ri.Name = "Section List";
                ri.Information = "The sections of the checked academic year and school.";
                ri.DisplayLevel = ReportLevel.Section;
            }
            else if (rt == ReportType.AddressType)
            {
                ri.Name = "Address Type List";
                ri.Information = "The address types for the address of the student, guardian, school, etc.";
            }
            else if (rt == ReportType.EmailType)
            {
                ri.Name = "Email Type List";
                ri.Information = "The email types for the email of the student, guardian, school, etc.";
            }
            else if (rt == ReportType.PhoneType)
            {
                ri.Name = "Phone Type List";
                ri.Information = "The phone types for the phone of the student, guardian, school, etc.";
            }
            else if (rt == ReportType.GuardianType)
            {
                ri.Name = "Guardian Type List";
                ri.Information = "The guardian types for the guardian.";
            }
            else if (rt == ReportType.Country)
            {
                ri.Name = "Country List";
                ri.Information = "The countries for the address, birth country of student and guardian.";
            }
            else if (rt == ReportType.Religion)
            {
                ri.Name = "Religion List";
                ri.Information = "The religions for the student, guardian, etc.";
            }
            else if (rt == ReportType.Currency)
            {
                ri.Name = "Currency List";
                ri.Information = "The currency kind of the payment.";
            }
            else if (rt == ReportType.FeeType)
            {
                ri.Name = "Fee Type List";
                ri.Information = "The fee types for the payment.";
            }

            return ri;
        }

        public static IEnumerable<ReportInfo> GetReportInfos(IEnumerable<ReportType> reportTypes)
        {
            return reportTypes.To(m => GetReportInfo(m));
        }

        public static IEnumerable<ReportInfo> GetSchoolReportInfos()
        {
            return GetSchoolReportTypes().To(m => GetReportInfo(m));
        }

        public static IEnumerable<ReportInfo> GetTypeReportInfos()
        {
            return GetTypeReportTypes().To(m => GetReportInfo(m));
        }

        public static IEnumerable<ReportInfo> GetAccountReportInfos()
        {
            return GetAccountReportTypes().To(m => GetReportInfo(m));
        }

        public static string ToReportLevelString(ReportLevel dl)
        {
            if (dl == ReportLevel.School)
                return "school";
            if (dl == ReportLevel.AcademicYear)
                return "academic year";
            if (dl == ReportLevel.Course)
                return "course";
            if (dl == ReportLevel.Class)
                return "class";
            if (dl == ReportLevel.Section)
                return "section";
            if (dl == ReportLevel.StudentSection)
                return "student";

            return string.Empty;
        }

        public static SMSDataSet CreateDataSet()
        {
            return new SMSDataSet();
        }

        public static void Add(this SMSDataSet ds, IEnumerable<AddressType> source, bool clear = true)
        {
            if (clear)
                ds.AddressTypes.Clear();
            foreach (var value in source)
                ds.AddressTypes.AddAddressTypesRow(value.AddressTypeId, value.AddressTypeName);
        }

        public static void Add(this SMSDataSet ds, IEnumerable<EmailType> source, bool clear = true)
        {
            if (clear)
                ds.EmailTypes.Clear();
            foreach (var value in source)
                ds.EmailTypes.AddEmailTypesRow(value.EmailTypeId, value.EmailTypeName);
        }

        public static void Add(this SMSDataSet ds, IEnumerable<PhoneType> source, bool clear = true)
        {
            if (clear)
                ds.PhoneTypes.Clear();
            foreach (var value in source)
                ds.PhoneTypes.AddPhoneTypesRow(value.PhoneTypeId, value.PhoneTypeName);
        }

        public static void Add(this SMSDataSet ds, IEnumerable<GuardianType> source, bool clear = true)
        {
            if (clear)
                ds.GuardianTypes.Clear();
            foreach (var value in source)
                ds.GuardianTypes.AddGuardianTypesRow(value.GuardianTypeId, value.GuardianTypeName, value.Gender);
        }

        public static void Add(this SMSDataSet ds, IEnumerable<Country> source, bool clear = true)
        {
            if (clear)
                ds.Countries.Clear();
            foreach (var value in source)
                ds.Countries.AddCountriesRow(value.CountryId, value.CountryName, value.CountryCode, value.Code);
        }

        public static void Add(this SMSDataSet ds, IEnumerable<Religion> source, bool clear = true)
        {
            if (clear)
                ds.Religions.Clear();
            foreach (var value in source)
                ds.Religions.AddReligionsRow(value.ReligionId, value.ReligionName);
        }

        public static void Add(this SMSDataSet ds, IEnumerable<Currency> source, bool clear = true)
        {
            if (clear)
                ds.Currencies.Clear();
            foreach (var value in source)
                ds.Currencies.AddCurrenciesRow(value.CurrencyId, value.CurrencyName, value.CurrencyCode, value.CurrencySymbol);
        }

        public static void Add(this SMSDataSet ds, IEnumerable<FeeType> source, bool clear = true)
        {
            if (clear)
                ds.FeeTypes.Clear();
            foreach (var value in source)
                ds.FeeTypes.AddFeeTypesRow(value.FeeTypeId, value.FeeTypeName);
        }

        public static void Add(this SMSDataSet ds, IEnumerable<School> source, bool clear = true)
        {
            if (clear)
                ds.Schools.Clear();
            foreach (var value in source)
                ds.Schools.AddSchoolsRow(value.SchoolId, value.AddressId, value.SchoolName, value.Motto,
                    value.Active);
        }

        public static void Add(this SMSDataSet ds, IEnumerable<AcademicYear> source, bool clear = true)
        {
            if (clear)
                ds.AcademicYears.Clear();
            foreach (var value in source)
                ds.AcademicYears.AddAcademicYearsRow(value.AcademicYearId, value.SchoolId,
                    value.AcademicYearName, value.Active);
        }

        public static void Add(this SMSDataSet ds, IEnumerable<Course> source, bool clear = true)
        {
            if (clear)
                ds.Courses.Clear();
            foreach (var value in source)
                ds.Courses.AddCoursesRow(value.CourseId, value.AcademicYearId, value.CourseName);
        }

        public static void Add(this SMSDataSet ds, IEnumerable<Class> source, bool clear = true)
        {
            if (clear)
                ds.Classes.Clear();
            foreach (var value in source)
                ds.Classes.AddClassesRow(value.ClassId, value.CourseId, value.ClassName);
        }

        public static void Add(this SMSDataSet ds, IEnumerable<Section> source, int sectionNameFormatIndex, bool clear = true)
        {
            if (clear)
                ds.Sections.Clear();
            var getSectionName = ModelExtension.GetSectionName(sectionNameFormatIndex);
            foreach (var value in source)
                ds.Sections.AddSectionsRow(value.SectionId, value.ClassId, getSectionName(value));
        }

        public static void Add(this SMSDataSet ds, IEnumerable<Student> source, bool clear = true)
        {
            if (clear)
                ds.Students.Clear();
            foreach (var value in source)
                ds.Students.AddStudentsRow(value.StudentId, value.Id, value.Title, value.StudentName,
                    value.OtherName, value.Gender, value.BirthDate.ToDateString(), value.Nationality,
                    value.NIRC, value.Passport, value.Visa, value.Description,
                    ds.ToCountryString(value.BirthCountryId), ds.ToReligionString(value.ReligionId));
        }

        public static void Add(this SMSDataSet ds, IEnumerable<StudentSection> source, bool clear = true)
        {
            if (clear)
                ds.StudentSections.Clear();
            foreach (var value in source)
                ds.StudentSections.AddStudentSectionsRow(value.StudentSectionId, value.StudentId,
                    value.SectionId, value.RegisteredDate.ToDateString(),
                    value.ResignedDate.ToDateString(), value.Active);
        }

        public static void Add(this SMSDataSet ds, IEnumerable<Guardian> source, bool clear = true)
        {
            if (clear)
                ds.Guardians.Clear();
            foreach (var value in source)
                ds.Guardians.AddGuardiansRow(value.GuardianId, value.Title, value.GuardianName,
                    value.Gender, value.BirthDate.ToDateString(), value.Nationality, value.NIRC,
                    value.Education, value.Employment, value.Designation, value.Passport, value.Visa,
                    ds.ToCountryString(value.BirthCountryId), ds.ToReligionString(value.ReligionId));
        }

        public static void AddSectionDetails(this SMSDataSet ds, IEnumerable<Section> source, bool clear = true)
        {
            if (clear)
                ds.SectionDetails.Clear();
            foreach (var value in source)
                ds.SectionDetails.AddSectionDetailsRow(value.SectionId, value.ClassId, value.SectionName,
                    value.Room, value.OpenDay, value.StartEndTime, value.CommencingCompletionDate);
        }

        public static void AddStudentList(this SMSDataSet ds, IEnumerable<StudentSection> source, int studentNameFormatIndex, bool clear = true)
        {
            if (clear)
                ds.StudentList.Clear();

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);

            foreach (var value in source)
                ds.StudentList.AddStudentListRow(value.StudentSectionId, value.SectionId,
                    value.Student.Id, getStudentName(value.Student), value.Student.Gender);
        }

        public static void AddStudentListAge(this SMSDataSet ds, IEnumerable<StudentSection> source, int studentNameFormatIndex, DateTime td, bool clear = true)
        {
            if (clear)
                ds.StudentListAge.Clear();

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);

            foreach (var value in source)
                ds.StudentListAge.AddStudentListAgeRow(value.StudentSectionId, value.SectionId,
                    value.Student.Id, getStudentName(value.Student),
                    value.Student.Gender, value.Student.BirthDate.ToDateString(),
                    value.Student.BirthDate.ToAgeString(td), value.Active);
        }

        public static void AddStudentListDetails(this SMSDataSet ds, IEnumerable<StudentSection> source, int studentNameFormatIndex, bool clear = true)
        {
            if (clear)
                ds.StudentListDetails.Clear();

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);

            foreach (var value in source)
                ds.StudentListDetails.AddStudentListDetailsRow(value.StudentSectionId, value.SectionId,
                    value.Student.Id, getStudentName(value.Student), value.Student.Gender,
                    value.RegisteredDate.ToDateString(), value.ResignedDate.ToDateString(), value.Active);
        }

        public static void AddStudentListInfo(this SMSDataSet ds, IEnumerable<StudentSection> source, bool allAddresses, bool allEmails, bool allPhones, List<Guid> guardianTypeIds, int studentNameFormatIndex, int addressFormatIndex, int guardianFormatIndex, bool clear = true)
        {
            if (clear)
                ds.StudentListInfo.Clear();

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);

            foreach (var value in source)
                ds.StudentListInfo.AddStudentListInfoRow(value.StudentSectionId, value.SectionId,
                    value.Student.Id, getStudentName(value.Student),
                    value.Student.StudentAddresses.ToAddressString(addressFormatIndex, allAddresses),
                    value.Student.StudentEmails.ToEmailString(allEmails),
                    value.Student.StudentPhones.ToPhoneString(allPhones),
                    value.Student.StudentGuardians.ToGuaridanString(guardianTypeIds, guardianFormatIndex));
        }

        public static void AddStudentListAddress(this SMSDataSet ds, IEnumerable<StudentSection> source, bool allAddresses, int studentNameFormatIndex, int addressFormatIndex, bool clear = true)
        {
            if (clear)
                ds.StudentList2.Clear();

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);

            foreach (var value in source)
                ds.StudentList2.AddStudentList2Row(value.StudentSectionId, value.SectionId,
                    value.Student.Id, getStudentName(value.Student),
                    value.Student.StudentAddresses.ToAddressString(addressFormatIndex, allAddresses));
        }

        public static void AddStudentListEmail(this SMSDataSet ds, IEnumerable<StudentSection> source, bool allEmails, int studentNameFormatIndex, bool clear = true)
        {
            if (clear)
                ds.StudentList2.Clear();

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);

            foreach (var value in source)
                ds.StudentList2.AddStudentList2Row(value.StudentSectionId, value.SectionId,
                    value.Student.Id, getStudentName(value.Student),
                    value.Student.StudentEmails.ToEmailString(allEmails));
        }

        public static void AddStudentListPhone(this SMSDataSet ds, IEnumerable<StudentSection> source, bool allPhones, int studentNameFormatIndex, bool clear = true)
        {
            if (clear)
                ds.StudentList2.Clear();

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);

            foreach (var value in source)
                ds.StudentList2.AddStudentList2Row(value.StudentSectionId, value.SectionId,
                    value.Student.Id, getStudentName(value.Student),
                    value.Student.StudentPhones.ToPhoneString(allPhones));
        }

        public static void AddStudentListGuardian(this SMSDataSet ds, IEnumerable<StudentSection> source, List<Guid> guardianTypeIds, int studentNameFormatIndex, int guardianFormatIndex, bool clear = true)
        {
            if (clear)
                ds.StudentList2.Clear();

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);

            foreach (var value in source)
                ds.StudentList2.AddStudentList2Row(value.StudentSectionId, value.SectionId,
                    value.Student.Id, getStudentName(value.Student),
                    value.Student.StudentGuardians.ToGuaridanString(guardianTypeIds, guardianFormatIndex));
        }

        public static void AddStudentCard(this SMSDataSet ds, IEnumerable<StudentSection> source, int studentNameFormatIndex, bool clear = true)
        {
            if (clear)
                ds.StudentCard.Clear();

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);

            foreach (var value in source)
                ds.StudentCard.AddStudentCardRow(value.StudentSectionId, value.SectionId,
                    value.Student.Id, getStudentName(value.Student));
        }

        public static void AddStudentDiscount(this SMSDataSet ds, IEnumerable<StudentDiscount> source, Guid courseId, int studentNameFormatIndex, int sectionNameFormatIndex, bool clear = true)
        {
            if (clear)
                ds.StudentDiscount.Clear();

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);
            var getSectionName = ModelExtension.GetSectionName(sectionNameFormatIndex);

            foreach (var value in source)
                ds.StudentDiscount.AddStudentDiscountRow(value.StudentSectionId,
                    courseId, value.StudentSection.Student.Id,
                    getStudentName(value.StudentSection.Student),
                    getSectionName(value.StudentSection.Section),
                    value.GetDiscountPercentage(), value.DiscountAmount);
        }

        public static void AddStudentFerryFee(this SMSDataSet ds, IEnumerable<StudentFerryFee> source, Guid courseId, int studentNameFormatIndex, int sectionNameFormatIndex, bool clear = true)
        {
            if (clear)
                ds.StudentFerryFee.Clear();

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);
            var getSectionName = ModelExtension.GetSectionName(sectionNameFormatIndex);

            foreach (var value in source)
                ds.StudentFerryFee.AddStudentFerryFeeRow(value.StudentSectionId,
                    courseId, value.StudentSection.Student.Id,
                    getStudentName(value.StudentSection.Student),
                    getSectionName(value.StudentSection.Section),
                    value.Fee, value.Monthly);
        }

        public static void AddReminder(this SMSDataSet ds, IEnumerable<StudentSection> source, int studentNameFormatIndex, bool clear = true)
        {
            if (clear)
                ds.Reminder.Clear();

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);

            foreach (var value in source)
                ds.Reminder.AddReminderRow(value.StudentSectionId, value.SectionId,
                    value.Student.Id, getStudentName(value.Student),
                    value.Section.Fee.GetValueOrDefault(0), value.StudentFerryFee.GetFee(0),
                    value.StudentDiscount.GetDiscountPercentage(0),
                    value.StudentDiscount.GetDiscountAmount(0) * -1);
        }

        public static void AddDailyIncome(this SMSDataSet ds, IEnumerable<Receipt> source, bool clear = true)
        {
            if (clear)
                ds.DailyIncome.Clear();
            foreach (var value in source)
            {
                var row = (SMSDataSet.DailyIncomeRow)ds.DailyIncome.NewRow();
                object[] array = new object[] { value.ReceiptId, value.CurrencyId, value.SchoolId,
                    value.ReceiptCodeNo, value.TotalAmount, value.TaxAmount, value.NetAmount };

                row.ItemArray = array;
                ds.DailyIncome.AddDailyIncomeRow(row);
            }
        }

        public static void AddDailyIncomeDetails(this SMSDataSet ds, IEnumerable<Receipt> source, int studentNameFormatIndex, int sectionNameFormatIndex, bool clear = true)
        {
            if (clear)
            {
                ds.DIReceipts.Clear();
                ds.DIReceiptDetails.Clear();
            }

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);
            var getSectionName = ModelExtension.GetSectionName(sectionNameFormatIndex);
            string format = "MMM-yyyy", separator = " to ";

            foreach (var value in source)
            {
                var row = (SMSDataSet.DIReceiptsRow)ds.DIReceipts.NewRow();
                object[] array = new object[] { value.ReceiptId, value.CurrencyId, value.SchoolId,
                    value.ReceiptCodeNo, value.TotalAmount, value.TaxAmount, value.NetAmount };

                row.ItemArray = array;
                ds.DIReceipts.AddDIReceiptsRow(row);

                foreach (var value2 in value.ReceiptDetails)
                {
                    var row2 = (SMSDataSet.DIReceiptDetailsRow)ds.DIReceiptDetails.NewRow();
                    object[] array2 = new object[] { value2.ReceiptDetailId, value2.ReceiptId,
                        value2.FeeTypeId, value2.StudentSection.Student.Id,
                        getStudentName(value2.StudentSection.Student),
                        getSectionName(value2.StudentSection.Section),
                        value2.GetMonth(format, separator),
                        value2.Quantity, value2.Amount, value2.DiscountAmount, value2.NetAmount };

                    row2.ItemArray = array2;
                    ds.DIReceiptDetails.AddDIReceiptDetailsRow(row2);
                }
            }
        }

        public static void AddMonthlyIncome(this SMSDataSet ds, IEnumerable<Receipt> source, List<Guid> currencyList, List<Guid> schoolList, bool clear = true)
        {
            if (clear)
                ds.MonthlyIncome.Clear();
            foreach (var cg in source.GroupBy(m => m.CurrencyId))
            {
                currencyList.Add(cg.Key);
                foreach (var sg in cg.GroupBy(m => m.SchoolId))
                {
                    schoolList.Add(sg.Key);
                    foreach (var value in sg.GroupBy(m => m.ReceiptDate))
                    {
                        var row = (SMSDataSet.MonthlyIncomeRow)ds.MonthlyIncome.NewRow();
                        object[] array = new object[] { value.Key.ToDateString(), cg.Key, sg.Key,
                            value.Sum(m => m.TotalAmount), value.Sum(m => m.TaxAmount),
                            value.Sum(m => m.NetAmount) };

                        row.ItemArray = array;
                        ds.MonthlyIncome.AddMonthlyIncomeRow(row);
                    }
                }
            }
        }

        public static void AddYearlyIncome(this SMSDataSet ds, IEnumerable<Receipt> source, List<Guid> currencyList, List<Guid> schoolList, bool clear = true)
        {
            if (clear)
                ds.YearlyIncome.Clear();
            foreach (var cg in source.GroupBy(m => m.CurrencyId))
            {
                currencyList.Add(cg.Key);
                foreach (var sg in cg.GroupBy(m => m.SchoolId))
                {
                    schoolList.Add(sg.Key);
                    foreach (var yg in sg.GroupBy(m => m.ReceiptDate.Year))
                    {
                        foreach (var value in yg.GroupBy(m => m.ReceiptDate.Month))
                        {
                            var row = (SMSDataSet.YearlyIncomeRow)ds.YearlyIncome.NewRow();
                            object[] array = new object[] { 
                                new DateTime(yg.Key, value.Key, 1).ToString("MMMM yyyy"),
                                cg.Key, sg.Key, value.Sum(m => m.TotalAmount), value.Sum(m => m.TaxAmount),
                                value.Sum(m => m.NetAmount) };

                            row.ItemArray = array;
                            ds.YearlyIncome.AddYearlyIncomeRow(row);
                        }
                    }
                }
            }
        }

        public static void AddReceipts(this SMSDataSet ds, IEnumerable<Receipt> source, int studentNameFormatIndex, int sectionNameFormatIndex, bool clear = true)
        {
            if (clear)
            {
                ds.Receipts.Clear();
                ds.ReceiptDetails.Clear();
            }

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);
            var getSectionName = ModelExtension.GetSectionName(sectionNameFormatIndex);
            string format = "MMM-yyyy", separator = " to ";

            foreach (var value in source)
            {
                var row = (SMSDataSet.ReceiptsRow)ds.Receipts.NewRow();
                object[] array = new object[] { value.ReceiptId, value.CurrencyId, value.SchoolId,
                    value.ReceiptDate.ToDateString(), value.ReceiptCodeNo, value.TotalAmount, value.TaxAmount, value.NetAmount };

                row.ItemArray = array;
                ds.Receipts.AddReceiptsRow(row);

                foreach (var value2 in value.ReceiptDetails)
                {
                    var row2 = (SMSDataSet.ReceiptDetailsRow)ds.DIReceiptDetails.NewRow();
                    object[] array2 = new object[] { value2.ReceiptDetailId, value2.ReceiptId,
                        value2.FeeTypeId, value2.StudentSectionId, value2.StudentSection.Student.Id,
                        getStudentName(value2.StudentSection.Student),
                        getSectionName(value2.StudentSection.Section),
                        value2.GetMonth(format, separator),
                        value2.Quantity, value2.Amount, value2.DiscountAmount, value2.NetAmount };

                    row2.ItemArray = array2;
                    ds.ReceiptDetails.AddReceiptDetailsRow(row2);
                }
            }
        }

        public static void AddReceipts(this SMSDataSet ds, Receipt value, int studentNameFormatIndex, int sectionNameFormatIndex, bool clear = true)
        {
            if (clear)
            {
                ds.Receipts.Clear();
                ds.ReceiptDetails.Clear();
            }

            var getStudentName = ModelExtension.GetStudentName(studentNameFormatIndex);
            var getSectionName = ModelExtension.GetSectionName(sectionNameFormatIndex);
            string format = "MMM-yyyy", separator = " to ";

            var row = (SMSDataSet.ReceiptsRow)ds.Receipts.NewRow();
            object[] array = new object[] { value.ReceiptId, value.CurrencyId, value.SchoolId,
                value.ReceiptDate.ToDateString(), value.ReceiptCodeNo, value.TotalAmount, value.TaxAmount, value.NetAmount };

            row.ItemArray = array;
            ds.Receipts.AddReceiptsRow(row);

            foreach (var value2 in value.ReceiptDetails)
            {
                var row2 = (SMSDataSet.ReceiptDetailsRow)ds.ReceiptDetails.NewRow();
                object[] array2 = new object[] { value2.ReceiptDetailId, value2.ReceiptId,
                    value2.FeeTypeId, value2.StudentSectionId, value2.StudentSection.Student.Id,
                    getStudentName(value2.StudentSection.Student),
                    getSectionName(value2.StudentSection.Section),
                    value2.GetMonth(format, separator),
                    value2.Quantity, value2.Amount, value2.DiscountAmount, value2.NetAmount };

                row2.ItemArray = array2;
                ds.ReceiptDetails.AddReceiptDetailsRow(row2);
            }
        }

        public static void AddSchools(this SMSDataSet ds, IEnumerable<School> source, int addressFormatIndex, bool clear = true)
        {
            if (clear)
                ds.Schools.Clear();

            var getAddress = ModelExtension.GetAddress(addressFormatIndex);

            foreach (var value in source)
            {
                ds.Schools.AddSchoolsRow(value.SchoolId, value.AddressId,
                    value.SchoolName, Environment.NewLine.Combine(value.Motto,
                    getAddress(value.Address), value.SchoolPhones.ToPhoneString()),
                    value.Active);
            }
        }

        public static string ToCountryString(this SMSDataSet ds, Guid? id)
        {
            if (id.HasValue)
            {
                var row = ds.Countries.FirstOrDefault(m => m.CountryId == id);

                if (row == null)
                    return null;
                else
                    return row.CountryName;
            }

            return null;
        }

        public static string ToReligionString(this SMSDataSet ds, Guid? id)
        {
            if (id.HasValue)
            {
                var row = ds.Religions.FirstOrDefault(m => m.ReligionId == id.Value);

                if (row == null)
                    return null;
                else
                    return row.ReligionName;
            }

            return null;
        }

        public static void PrintReceipt(SMSDataSet ds, string printerName)
        {
            var rpt = new CRReceipt();

            rpt.SetDataSource(ds);
            rpt.PrintOptions.PrinterName = printerName;
            rpt.PrintToPrinter(1, false, 0, 0);
        }

        public static CRReceipt GetReceiptReport(SMSDataSet ds)
        {
            var rpt = new CRReceipt();

            rpt.SetDataSource(ds);

            return rpt;
        }
    }
}
