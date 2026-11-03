using CodeflexSystems.Library.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Licensing
{
    internal sealed class SMSLicenseInfo
    {
        internal ProductType ProductType { get; private set; }
        internal Version Version { get; private set; }
        internal LicenseType LicenseType { get; private set; }
        internal DateTime GenerateDate { get; private set; }
        internal DateTime? ActivationDate { get; private set; }
        internal DateTime? ServerActivationDate { get; private set; }
        internal ushort TrialDays { get; private set; }
        internal int DaysLeft { get; private set; }
        internal bool AllowMultiLanguage { get; set; }
        internal bool AllowMultiCurrency { get; set; }
        internal byte CurrenciesPerLicense { get; set; }
        internal bool AllowMultiSchool { get; set; }
        internal byte SchoolsPerLicense { get; set; }
        internal bool AllowMultiStudent { get; set; }
        internal ushort StudentsPerLicense { get; set; }

        internal LicenseStatus Status { get; private set; }
        internal string StatusReason { get; private set; }

        internal SMSLicenseInfo()
        {
            Reset();
        }

        internal SMSLicenseInfo(LicenseStatus status, string reason)
        {
            Reset();
            this.Status = status;
            this.StatusReason = reason;
        }

        internal SMSLicenseInfo(SMSLicenseKeyInfo ki)
        {
            this.ProductType = ki.ProductType;
            this.Version = ki.Version;
            this.LicenseType = ki.LicenseType;
            this.GenerateDate = ki.GenerateDate;
            this.TrialDays = ki.TrialDays;
            this.AllowMultiLanguage = ki.AllowMultiLanguage;
            this.AllowMultiCurrency = ki.AllowMultiCurrency;
            this.CurrenciesPerLicense = ki.CurrenciesPerLicense;
            this.AllowMultiSchool = ki.AllowMultiSchool;
            this.SchoolsPerLicense = ki.SchoolsPerLicense;
            this.AllowMultiStudent = ki.AllowMultiStudent;
            this.StudentsPerLicense = ki.StudentsPerLicense;

            //CheckProductType();
            //CheckLicenseType();
            if (!ReflectionHelper.IsValidEnum(this.ProductType))
                this.ProductType = ProductType.Invalid;
            if (!ReflectionHelper.IsValidEnum(this.LicenseType))
                this.LicenseType = LicenseType.Trial;
            if (this.ProductType == ProductType.Invalid || this.ProductType != ProductType.SMS)
            {
                this.Status = LicenseStatus.Invalid;
                this.StatusReason = SR2.GetString("LIIV");
                //License is invalid.
                return;
            }
            var ver = this.GetType().Assembly.GetName().Version;
            if (this.Version.Major != ver.Major || this.Version.Minor != ver.Minor)
            {
                this.Status = LicenseStatus.VersionMismatch;
                this.StatusReason = SR2.GetString("VIMM");
                //Version is mismatch.
                return;
            }
            if (LicenseHelper.BaseDate > this.GenerateDate)
            {
                this.Status = LicenseStatus.InternalError;
                this.StatusReason = SR2.GetString("GDIIV");
                //Generate date is invalid.
                return;
            }
            if (this.LicenseType == LicenseType.Trial)
            {
                //this.DaysLeft = (this.GenerateDate.AddDays(this.TrialDays) - DateTime.Today).Days;
                this.DaysLeft = this.TrialDays;
                if (this.DaysLeft > 0)
                {
                    this.Status = LicenseStatus.Trial;
                    this.StatusReason = SR2.GetString("DL", this.DaysLeft);
                    //{0} days left.
                }
                else
                {
                    this.DaysLeft = 0;
                    this.Status = LicenseStatus.Expired;
                    this.StatusReason = SR2.GetString("LIE");
                    //License is expired.
                }
            }
            else
            {
                this.DaysLeft = int.MaxValue;
                this.Status = LicenseStatus.Licensed;
                this.StatusReason = string.Empty;
            }
        }

        internal SMSLicenseInfo(ProductType ptype, int vmajor, int vminor, LicenseType ltype,
            DateTime gdate, DateTime? cadate, DateTime? sadate, ushort tdays, bool multilanguage,
            bool multicurrency, byte currenciesperlicense, bool multischool, byte schoolsperlicense,
            bool multistudent, ushort studentsperlicense)
        {
            this.ProductType = ptype;
            this.Version = new Version(vmajor, vminor, 0, 0);
            this.LicenseType = ltype;
            this.GenerateDate = gdate;
            this.ActivationDate = cadate;
            this.ServerActivationDate = sadate;
            this.TrialDays = tdays;
            this.DaysLeft = 0;
            this.AllowMultiLanguage = multilanguage;
            this.AllowMultiCurrency = multicurrency;
            this.CurrenciesPerLicense = currenciesperlicense;
            this.AllowMultiSchool = multischool;
            this.SchoolsPerLicense = schoolsperlicense;
            this.AllowMultiStudent = multistudent;
            this.StudentsPerLicense = studentsperlicense;

            //CheckProductType();
            //CheckLicenseType();
            if (!ReflectionHelper.IsValidEnum(this.ProductType))
                this.ProductType = ProductType.Invalid;
            if (!ReflectionHelper.IsValidEnum(this.LicenseType))
                this.LicenseType = LicenseType.Trial;
            if (this.ProductType == ProductType.Invalid || this.ProductType != ProductType.SMS)
            {
                this.Status = LicenseStatus.Invalid;
                this.StatusReason = SR2.GetString("LIIV");
                //License is invalid.
                return;
            }
            if (LicenseHelper.BaseDate > this.GenerateDate)
            {
                this.Status = LicenseStatus.InternalError;
                this.StatusReason = SR2.GetString("GDIIV");
                //Generate date is invalid.
                return;
            }
            if (this.ActivationDate.HasValue == false ||
                (this.ActivationDate.HasValue && this.GenerateDate > this.ActivationDate))
            {
                this.Status = LicenseStatus.InternalError;
                this.StatusReason = SR2.GetString("ADIIV");
                //Activation date is invalid.
                return;
            }
            if (this.ServerActivationDate.HasValue == false ||
                (this.ServerActivationDate.HasValue && this.GenerateDate > this.ServerActivationDate))
            {
                this.Status = LicenseStatus.InternalError;
                this.StatusReason = SR2.GetString("SADIIV");
                //Server activation date is invalid.
                return;
            }
            if (this.LicenseType == LicenseType.Trial)
                UpdateDaysLeft();
            else
            {
                this.DaysLeft = int.MaxValue;
                this.Status = LicenseStatus.Licensed;
                this.StatusReason = null;
            }
        }

        internal void UpdateDaysLeft()
        {
            if (this.ProductType == ProductType.SMS && this.LicenseType == LicenseType.Trial
                && this.ActivationDate.HasValue)
            {
                int dl = this.TrialDays - (DateTime.Today - this.ActivationDate.Value.Date).Days;

                if (dl != this.DaysLeft)
                {
                    if (dl > 0 && dl <= this.TrialDays)
                    {
                        this.DaysLeft = dl;
                        this.Status = LicenseStatus.Trial;
                        this.StatusReason = SR2.GetString("DL", this.DaysLeft);
                        //{0} days left.
                    }
                    else
                    {
                        this.DaysLeft = 0;
                        this.Status = LicenseStatus.Expired;
                        this.StatusReason = SR2.GetString("LIE");
                        //License is expired.
                    }
                }
            }
        }

        internal bool IsValid()
        {
            if (this.ProductType != ProductType.SMS)
                return false;

            UpdateDaysLeft();

            return this.Status == LicenseStatus.Licensed || this.Status == LicenseStatus.Trial;
        }

        //private void CheckProductType()
        //{
        //    bool flag = this.ProductType == ProductType.Invalid ||
        //        this.ProductType == ProductType.SmartUI ||
        //        this.ProductType == ProductType.SMS;

        //    if (!flag)
        //        this.ProductType = ProductType.Invalid;
        //}

        //private void CheckLicenseType()
        //{
        //    bool flag = this.LicenseType == LicenseType.Trial ||
        //        this.LicenseType == LicenseType.Free ||
        //        this.LicenseType == LicenseType.Commercial ||
        //        this.LicenseType == LicenseType.Development;

        //    if (!flag)
        //        this.LicenseType = LicenseType.Trial;
        //}

        private void Reset()
        {
            this.ProductType = ProductType.Invalid;
            this.Version = new Version(0, 0, 0, 0);
            this.LicenseType = LicenseType.Trial;
            this.GenerateDate = new DateTime(1, 1, 1);
            this.ActivationDate = null;
            this.ServerActivationDate = null;
            this.TrialDays = 0;
            this.DaysLeft = 0;
            this.AllowMultiLanguage = false;
            this.AllowMultiCurrency = false;
            this.CurrenciesPerLicense = 0;
            this.AllowMultiSchool = false;
            this.SchoolsPerLicense = 0;
            this.AllowMultiStudent = false;
            this.StudentsPerLicense = 0;
            this.Status = LicenseStatus.Invalid;
            this.StatusReason = null;
        }
    }
}
