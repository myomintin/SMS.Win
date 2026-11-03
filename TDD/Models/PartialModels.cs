using CodeflexSystems.Library.Extensions;
using SMS.Extensions;
using System;
using System.Collections.Generic;

namespace SMS.Models
{
    public partial class AcademicYear
    {
        public bool CanDelete
        {
            get { return this.Courses.Count == 0; }
        }

        public override string ToString()
        {
            if (this.AcademicYearName == null)
                return base.ToString();

            return this.AcademicYearName;
        }
    }

    public partial class Address
    {
        public string Line1City
        {
            get { return ", ".Combine(this.Line1, this.City); }
        }

        public string Line1Township
        {
            get { return ", ".Combine(this.Line1, this.Township); }
        }

        public string Line1CityTownship
        {
            get { return ", ".Combine(this.Line1, this.City, this.Township); }
        }

        public string LongAddress
        {
            get { return ", ".Combine(this.Line1, this.City, this.Township, this.Country == null ? string.Empty : this.Country.CountryName); }
        }

        public bool CanDelete
        {
            get
            {
                //return this.Schools.Count == 0 &&
                //    this.GuardianAddresses.Count == 0 &&
                //    this.StudentAddresses.Count == 0;
                return !(this.StudentAddresses.Count > 0 ||
                    this.GuardianAddresses.Count > 0 ||
                    this.Schools.Count > 0);
            }
        }

        public override string ToString()
        {
            if (this.Line1 == null)
                return base.ToString();

            return this.Line1;
        }
    }

    public partial class AddressType
    {
        public bool CanDelete
        {
            get
            {
                //return this.GuardianAddresses.Count == 0 &&
                //    this.StudentAddresses.Count == 0;
                return !(this.StudentAddresses.Count > 0 ||
                    this.GuardianAddresses.Count > 0);
            }
        }

        public override string ToString()
        {
            if (this.AddressTypeName == null)
                return base.ToString();

            return this.AddressTypeName;
        }
    }

    public partial class Class
    {
        public bool CanDelete
        {
            get { return this.Sections.Count == 0; }
        }

        public override string ToString()
        {
            if (this.ClassName == null)
                return base.ToString();

            return this.ClassName;
        }
    }

    public partial class Country
    {
        public bool CanDelete
        {
            get
            {
                //return this.Addresses.Count == 0 &&
                //    this.Guardians.Count == 0 &&
                //    this.Students.Count == 0;
                return !(this.Addresses.Count > 0 ||
                    this.Students.Count > 0 ||
                    this.Guardians.Count > 0);
            }
        }

        public override string ToString()
        {
            if (this.CountryName == null)
                return base.ToString();

            return this.CountryName;
        }
    }

    public partial class Course
    {
        public bool CanDelete
        {
            get { return this.Classes.Count == 0; }
        }

        public override string ToString()
        {
            if (this.CourseName == null)
                return base.ToString();

            return this.CourseName;
        }
    }

    public partial class Currency
    {
        public bool CanDelete
        {
            get { return false; }
        }

        public override string ToString()
        {
            if (this.CurrencyName == null)
                return base.ToString();

            return this.CurrencyName;
        }
    }

    public partial class EmailType
    {
        public bool CanDelete
        {
            get
            {
                //return this.GuardianEmails.Count == 0 &&
                //    this.SchoolEmails.Count == 0 &&
                //    this.StudentEmails.Count == 0;
                return !(this.StudentEmails.Count == 0 ||
                    this.GuardianEmails.Count == 0 ||
                    this.SchoolEmails.Count == 0);
            }
        }

        public override string ToString()
        {
            if (this.EmailTypeName == null)
                return base.ToString();

            return this.EmailTypeName;
        }
    }

    public partial class FeeType
    {
        public bool CanDelete
        {
            get { return this.ReceiptDetails.Count == 0; }
        }

        public override string ToString()
        {
            if (this.FeeTypeName == null)
                return base.ToString();

            return this.FeeTypeName;
        }
    }

    public partial class Guardian
    {
        public string TitleGuardianName
        {
            get { return " ".Combine(this.Title, this.GuardianName); }
        }

        public bool CanDelete
        {
            get { return this.StudentGuardians.Count == 0; }
        }

        public override string ToString()
        {
            if (this.GuardianName == null)
                return base.ToString();

            return this.GuardianName;
        }
    }

    public partial class GuardianType
    {
        public bool CanDelete
        {
            get { return this.StudentGuardians.Count == 0; }
        }

        public override string ToString()
        {
            if (this.GuardianTypeName == null)
                return base.ToString();

            return this.GuardianTypeName;
        }
    }

    public partial class PhoneType
    {
        public bool CanDelete
        {
            get
            {
                //return this.GuardianPhones.Count == 0 &&
                //    this.SchoolPhones.Count == 0 &&
                //    this.StudentPhones.Count == 0;
                return !(this.StudentPhones.Count > 0 ||
                    this.GuardianPhones.Count > 0 ||
                    this.SchoolPhones.Count > 0);
            }
        }

        public override string ToString()
        {
            if (this.PhoneTypeName == null)
                return base.ToString();

            return this.PhoneTypeName;
        }
    }

    public partial class Receipt
    {
        public string ReceiptCodeNo
        {
            get { return "-".Combine(this.ReceiptCode, this.ReceiptNo.ToString()); }
        }

        public bool CanDelete
        {
            get { return true; }
        }

        public override string ToString()
        {
            var rcn = this.ReceiptCodeNo;

            if (rcn == null)
                return base.ToString();

            return rcn;
        }

        public decimal ComputeNetAmount()
        {
            if (this.TaxAmount.HasValue)
                return this.TotalAmount + this.TaxAmount.Value;

            return this.TotalAmount;
        }

        public decimal? ComputeTaxAmount(decimal taxPercentage, decimal? defaultValue = null)
        {
            //if (this.TaxPercentage > 0 && this.TotalAmount > 0)
            //    return (this.TotalAmount / 100) * taxPercentage;

            if (taxPercentage > 0 && this.TotalAmount > 0)
                return taxPercentage * this.TotalAmount / 100;

            return defaultValue;
        }

        public decimal ComputeTaxPercentage(decimal defaultValue = 0)
        {
            //if (this.TaxAmount.HasValue && this.TotalAmount > 0)
            //    return (100 / this.TotalAmount) * this.TaxAmount.Value;

            if (this.TaxAmount.HasValue && this.TotalAmount > 0)
                return this.TaxAmount.Value * 100 / this.TotalAmount;

            return defaultValue;
        }

        public decimal ComputeTotalAmount()
        {
            decimal totalAmount = 0;

            foreach (var rd in this.ReceiptDetails)
                totalAmount += rd.NetAmount;

            return totalAmount;
        }
    }

    public partial class ReceiptDetail
    {
        public bool CanDelete
        {
            get { return true; }
        }

        public override string ToString()
        {
            return this.ReceiptDetailId.ToString();
        }

        public string GetMonth(string format, string separator)
        {
            if (this.FromMonth.HasValue)
            {
                var value = this.FromMonth.Value;

                if (this.Quantity == 1)
                    return value.ToString(format);
                else if (this.Quantity > 1)
                    return string.Format("{0}{1}{2}", value.ToString(format), separator,
                        value.AddMonths(this.Quantity).ToString(format));
            }

            return null;
        }

        public decimal? ComputeDiscountAmount(decimal discountPercentage, decimal? defaultValue = null)
        {
            decimal totalAmount = ComputeTotalAmount();

            //if (discountPercentage > 0 && totalAmount > 0)
            //    return (totalAmount / 100) * discountPercentage;

            if (discountPercentage > 0 && totalAmount > 0)
                return discountPercentage * totalAmount / 100;

            return defaultValue;
        }

        public decimal ComputeDiscountPercentage(bool withQuantity = true, decimal defaultValue = 0)
        {
            decimal totalAmount = withQuantity ? ComputeTotalAmount() : this.Amount;

            //if (this.DiscountAmount.HasValue && totalAmount > 0)
            //    return (100 / totalAmount) * this.DiscountAmount.Value;

            if (this.DiscountAmount.HasValue && totalAmount > 0)
                return this.DiscountAmount.Value * 100 / totalAmount;

            return defaultValue;
        }

        public decimal ComputeNetAmount()
        {
            decimal totalAmount = ComputeTotalAmount();

            if (this.DiscountAmount.HasValue && totalAmount > 0)
                return totalAmount - this.DiscountAmount.Value;

            return totalAmount;
        }

        public decimal ComputeTotalAmount()
        {
            return this.Amount * this.Quantity;
        }
    }

    public partial class Religion
    {
        public bool CanDelete
        {
            get
            {
                //return this.Guardians.Count == 0 &&
                //    this.Students.Count == 0;
                return !(this.Students.Count > 0 ||
                    this.Guardians.Count == 0);
            }
        }

        public override string ToString()
        {
            if (this.ReligionName == null)
                return base.ToString();

            return this.ReligionName;
        }
    }

    public partial class School
    {
        public bool CanDelete
        {
            get { return this.AcademicYears.Count == 0; }
        }

        public override string ToString()
        {
            if (this.SchoolName == null)
                return base.ToString();

            return this.SchoolName;
        }
    }

    public partial class Section
    {
        public string SectionNameWithOpenDay
        {
            get
            {
                if (string.IsNullOrEmpty(this.OpenDay))
                    return this.SectionName;

                return string.Format("{0} [ {1} ]", this.SectionName, this.OpenDay);
            }
        }

        public string SectionNameWithStartEndTime
        {
            get
            {
                var value = this.StartEndTime;

                if (string.IsNullOrEmpty(value))
                    return this.SectionName;

                return string.Format("{0} [ {1} ]", this.SectionName, value);
            }
        }

        public string SectionNameWithOpenDayStartEndTime
        {
            get
            {
                var value = this.OpenDayStartEndTime;

                if (string.IsNullOrEmpty(value))
                    return this.SectionName;

                return string.Format("{0} [ {1} ]", this.SectionName, value);
            }
        }

        public string OpenDayStartEndTime
        {
            get
            {
                return ", ".Combine(this.OpenDay, this.StartEndTime);
            }
        }

        public string StartEndTime
        {
            get
            {
                return " ~ ".Combine(this.StartTime.ToTimeString(), this.EndTime.ToTimeString());
            }
        }

        public string CommencingCompletionDate
        {
            get
            {
                return " ~ ".Combine(this.CommencingDate.ToDateString(), this.CompletionDate.ToDateString());
            }
        }

        public bool CanDelete
        {
            get { return this.StudentSections.Count == 0; }
        }

        public override string ToString()
        {
            if (this.SectionName == null)
                return base.ToString();

            return this.SectionName;
        }
    }

    public partial class Student
    {
        public string TitleStudentFullName
        {
            get { return " ".Combine(this.Title, this.StudentFullName); }
        }

        public string StudentFullName
        {
            get { return " @ ".Combine(this.StudentName, this.OtherName); }
        }

        public bool CanDelete
        {
            get { return this.StudentSections.Count == 0; }
        }

        public override string ToString()
        {
            var sfn = this.StudentFullName;

            if (sfn == null)
                return base.ToString();

            return sfn;
        }
    }

    public partial class StudentSection
    {
        public bool CanDelete
        {
            get
            {
                //return this.ReceiptDetails.Count == 0;
                return !(this.ReceiptDetails.Count > 0 ||
                    this.StudentDiscount != null ||
                    this.StudentFerryFee != null);
            }
        }

        public override string ToString()
        {
            return this.StudentSectionId.ToString();
        }
    }

    public partial class StudentDiscount
    {
        public bool CanDelete
        {
            get { return true; }
        }

        public override string ToString()
        {
            return this.StudentSectionId.ToString();
        }

        public decimal ComputeDiscountPercentage(decimal defaultValue = 0)
        {
            decimal? fee = this.StudentSection.Section.Fee;

            if (fee.HasValue)
                return this.DiscountAmount * 100 / fee.Value;

            return defaultValue;
        }
    }

    public partial class StudentFerryFee
    {
        public bool CanDelete
        {
            get { return true; }
        }

        public override string ToString()
        {
            return this.StudentSectionId.ToString();
        }
    }

    public partial class User
    {
        public bool CanDelete
        {
            get { return false; }
        }

        public override string ToString()
        {
            if (this.UserName == null)
                return base.ToString();

            return this.UserName;
        }
    }

    public partial class UserMembership
    {
        public bool CanDelete
        {
            get { return false; }
        }
    }

    public partial class UserOption
    {
        public bool CanDelete
        {
            get { return false; }
        }

        public override string ToString()
        {
            return this.UserId.ToString();
        }
    }

    public partial class UserPermission
    {
        public bool CanDelete
        {
            get { return true; }
        }

        public bool IsAllowed(Permission value)
        {
            if (value == Permission.None)
                return this.Permission == value;

            return (this.Permission & value) == value;
        }

        public void SetPermission(Permission value, bool flag)
        {
            if (flag)
            {
                if (value == Permission.None)
                    this.Permission = Permission.None;
                else
                    this.Permission = this.Permission | value;
            }
            else
                this.Permission = this.Permission & ~value;
        }
    }
}
