using System;
using System.Text;

namespace CodeflexSystems.SMS.Licensing
{
    internal class LicenseKeyInfo
    {
        internal ProductType ProductType { get; set; }
        internal Version Version { get; set; }
        internal LicenseType LicenseType { get; set; }
        internal DateTime GenerateDate { get; set; }
        internal ushort TrialDays { get; set; }
    }

    internal sealed class SMSLicenseKeyInfo : LicenseKeyInfo
    {
        internal bool AllowMultiLanguage { get; set; }
        internal bool AllowMultiCurrency { get; set; }
        internal byte CurrenciesPerLicense { get; set; }
        internal bool AllowMultiSchool { get; set; }
        internal byte SchoolsPerLicense { get; set; }
        internal bool AllowMultiStudent { get; set; }
        internal ushort StudentsPerLicense { get; set; }
    }
}
