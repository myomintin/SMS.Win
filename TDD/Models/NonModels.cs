using CodeflexSystems.Library.Extensions;
using System;
using System.Collections.Generic;

namespace SMS.Models
{
    // Those classes are not DB models.

    public class SectionInfo
    {
        public Section Section { get; set; }
        public Class Class { get; set; }
        public Course Course { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public School School { get; set; }
    }

    public class StudentSectionInfo
    {
        public StudentSection StudentSection { get; set; }
        public Student Student { get; set; }
        public Section Section { get; set; }
        public Class Class { get; set; }
        public Course Course { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public School School { get; set; }
    }

    public class CurrencyInfo
    {
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsSelected { get; set; }
        public bool Active { get; set; }

        public string CurrencyCodeName
        {
            get { return " - ".Combine(this.CurrencyCode, this.CurrencyName); }
        }
    }

    public class CountryInfo
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string Code { get; set; }
        public bool IsSelected { get; set; }
        public bool Active { get; set; }
    }

    [Flags]
    public enum Permission : short
    {
        None = 0,
        CanView = 1,
        CanAdd = 2,
        CanEdit = 4,
        CanDelete = 8,
        CanPrint = 16,
        CanResetPassword = 32,
        CanSetPermission = 64
    }
}
