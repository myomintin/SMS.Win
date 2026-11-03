using CodeflexSystems.SMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Licensing
{
    internal sealed class LicenseHelper
    {
        #region Fields

        private static IEnumerable<uint> LText
        {
            get
            {
                yield return 13288;
                yield return 11816;
                yield return 13288;
                yield return 2053;
                yield return 11729;
                yield return 11088;
                yield return 9736;
                yield return 10136;
                yield return 12037;
                yield return 13288;
                yield return 10136;
                yield break;
            }
        }

        internal static readonly DateTime BaseDate;
        private SMSLicenseInfo license;

        #endregion

        #region Properties

        internal static LicenseHelper Current { get; private set; }

        internal SMSLicenseInfo License
        {
            get { return this.license; }
            set
            {
                if (this.license != value)
                {
                    this.license = value;
                    LicenseChanged();
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        static LicenseHelper()
        {
            BaseDate = new DateTime(2017, 1, 1);
            Current = new LicenseHelper();
        }

        internal LicenseHelper()
        {
            LicenseChanged();
        }

        #endregion

        #region Methods

        private void LicenseChanged()
        {
#if DEBUG
            if (ApplicationManager.Current.IsFakeLogon)
            {
                ApplicationManager.a = true;
                ApplicationManager.b = false;
                ApplicationManager.c = 1;
                ApplicationManager.d = false;
                ApplicationManager.e = 1;
                ApplicationManager.f = false;
                ApplicationManager.g = 392;

                return;
            }
            if (IsValid())
            {
                ApplicationManager.a = this.license.AllowMultiLanguage;
                ApplicationManager.b = this.license.AllowMultiCurrency;
                ApplicationManager.c = this.license.CurrenciesPerLicense;
                ApplicationManager.d = this.license.AllowMultiSchool;
                ApplicationManager.e = this.license.SchoolsPerLicense;
                ApplicationManager.f = this.license.AllowMultiStudent;
                ApplicationManager.g = this.license.StudentsPerLicense;
            }
            else
            {
                ApplicationManager.a = false;
                ApplicationManager.b = false;
                ApplicationManager.c = 0;
                ApplicationManager.d = false;
                ApplicationManager.e = 0;
                ApplicationManager.f = false;
                ApplicationManager.g = 0;
            }
#else
            if (IsValid())
            {
                ApplicationManager.a = this.license.AllowMultiLanguage;
                ApplicationManager.b = this.license.AllowMultiCurrency;
                ApplicationManager.c = this.license.CurrenciesPerLicense;
                ApplicationManager.d = this.license.AllowMultiSchool;
                ApplicationManager.e = this.license.SchoolsPerLicense;
                ApplicationManager.f = this.license.AllowMultiStudent;
                ApplicationManager.g = this.license.StudentsPerLicense;
            }
            else
            {
                ApplicationManager.a = false;
                ApplicationManager.b = false;
                ApplicationManager.c = 0;
                ApplicationManager.d = false;
                ApplicationManager.e = 0;
                ApplicationManager.f = false;
                ApplicationManager.g = 0;
            }
#endif
        }

        internal void CheckLicense()
        {
            LoadLicense(false);
            if (!IsValid())
            {
                TD.ShowErrorAndLog(SR2.GetString("LIC"), LicenseHelper.Current.GetStatusReason(),
                    path: "#LH#", member: "CL");
                SetLicense();
            }
        }

        internal void LoadLicense(bool showError = true)
        {
            try
            {
                if (this.license == null)
                {
                    var ldi = ResourceHelper.Load(GetL());

                    if (ldi == null)
                        ThrowNoLicense();
                    this.License = LicenseManager.GetSMSLI(ldi);
                }
            }
            catch (Exception ex)
            {
                if (showError)
                    TD.ShowErrorAndLog(SR2.GetString("LIC"), ex, path: "#LH#", member: "LL");
            }
        }

        internal bool HasLicense()
        {
            return this.license != null;
        }

        internal bool IsValid()
        {
            if (this.license == null)
                return false;

            return this.license.IsValid();
        }

        internal void SetLicense()
        {
            try
            {
                var step = NavigationManager.ShowLRCS();

                if (step.DialogResult != true)
                    return;

                if (step.KRadioButton.IsChecked == true)
                {
                    var step1 = NavigationManager.ShowLRSO();

                    if (step1.DialogResult != true)
                        return;

                    var k = step1.KTextBox.Text;
                    var l = Create(k);

                    if (!l.IsValid())
                    {
                        TD.ShowErrorAndLog(SR2.GetString("LIC"), l.StatusReason,
                            path: "#LH#", member: "SL");
                        return;
                    }

                    var step2 = NavigationManager.ShowLRST();

                    if (step2.DialogResult != true)
                        return;
                    Save(step2.AFileSaver.SelectedFile, l);
                }

                var step3 = NavigationManager.ShowLRSTH();

                if (step3.DialogResult != true)
                    return;

                this.License = DoActivation(step3.SFilePicker.SelectedFile);
                if (IsValid())
                    TD.Show(SR2.GetString("LIC"), SR2.GetString("LRISC"));
                else
                    TD.ShowErrorAndLog(SR2.GetString("LIC"), SR2.GetString("LRIIPRA"),
                        path: "#LH#", member: "SL");
            }
            catch (Exception ex)
            {
                TD.ShowErrorAndLog(SR2.GetString("LIC"), ex, path: "#LH#", member: "SL");
            }
        }

        internal string GetStatusReason()
        {
            if (this.license == null)
                return SR2.GetString("LINFPRL");

            return string.IsNullOrEmpty(this.license.StatusReason) ?
                SR2.GetString("LIIV") : this.license.StatusReason;
        }

        internal string GetLicenseType()
        {
            if (HasLicense())
            {
                if (this.license.LicenseType == LicenseType.Trial)
                    return SR2.GetString("LIC2", SR2.GetString("LICT"), this.license.StatusReason);
                else if (this.license.LicenseType == LicenseType.Free)
                    return SR2.GetString("LIC1", SR2.GetString("LICF"));
                else if (this.license.LicenseType == LicenseType.Commercial)
                    return SR2.GetString("LIC1", SR2.GetString("LICC"));
                else if (this.license.LicenseType == LicenseType.Development)
                    return SR2.GetString("LIC1", SR2.GetString("LICD"));
                else
                    return SR2.GetString("ULIC");
            }

            return SR2.GetString("ULIC");
        }

        #endregion

        #region Static Methods

        internal static SMSLicenseInfo Create(string key)
        {
            var ki = LicenseManager.VerifySMS(key);

            return new SMSLicenseInfo(ki);
        }

        internal static void Save(string path, SMSLicenseInfo lic)
        {
            if (lic == null)
                ThrowLicenseIsNull();
            if (!lic.IsValid())
                ThrowLicense(lic);

            var ldi = LicenseManager.GetLDI(lic);

            ResourceHelper.Save(path, ldi);
        }

        internal static SMSLicenseInfo DoActivation(string path)
        {
            var ldi = ResourceHelper.Load(path);
            var lic = LicenseManager.GetSMSLI(ldi);

            if (lic.IsValid())
                ResourceHelper.Save(GetL(), ldi);

            return lic;
        }

        internal static string GetMultiLanguage()
        {
            if (ApplicationManager.a)
                return SR2.GetString("AML");
            else
                return SR2.GetString("NAML");
        }

        internal static string GetMultiCurrency()
        {
            if (ApplicationManager.b)
                return SR2.GetString("AMC");
            else
                return SR2.GetString("NAMC", ApplicationManager.c);
        }

        internal static string GetMultiSchool()
        {
            if (ApplicationManager.d)
                return SR2.GetString("AMS");
            else
                return SR2.GetString("NAMS", ApplicationManager.e);
        }

        internal static string GetMultiStudent()
        {
            if (ApplicationManager.f)
                return SR2.GetString("AMT");
            else
                return SR2.GetString("NAMT", ApplicationManager.g);
        }

        internal static string GetLRIIASLP()
        {
            return SR2.GetString("LRIIASLP", Environment.NewLine);
        }

        internal static string GetLIC()
        {
            return SR2.GetString("LIC");
        }

        /// <summary>
        /// Currencies / License
        /// </summary>
        /// <returns></returns>
        internal static string GetCLCPL()
        {
            return SR2.GetString("CLCPL", ApplicationManager.c);
        }

        /// <summary>
        /// Schools / License
        /// </summary>
        /// <returns></returns>
        internal static string GetCLSPL()
        {
            return SR2.GetString("CLSPL", ApplicationManager.e);
        }

        /// <summary>
        /// Students / License
        /// </summary>
        /// <returns></returns>
        internal static string GetCLTPL()
        {
            return SR2.GetString("CLTPL", ApplicationManager.g);
        }

        internal static string GetIUWAMPUL(string msg)
        {
            return SR2.GetString("IUWAMPUL", msg);
        }

        /// <summary>
        /// Throw Currencies / License
        /// </summary>
        internal static void ThrowLicenseCLCPL()
        {
            throw new SMSException(GetCLCPL(), new Exception(SR2.GetString("IUWAMPUL", "currencies")));
        }

        /// <summary>
        /// Throw Schools / License
        /// </summary>
        internal static void ThrowLicenseCLSPL()
        {
            throw new SMSException(GetCLSPL(), new Exception(SR2.GetString("IUWAMPUL", "schools")));
        }

        /// <summary>
        /// Throw Students / License
        /// </summary>
        internal static void ThrowLicenseCLTPL()
        {
            throw new SMSException(GetCLTPL(), new Exception(SR2.GetString("IUWAMPUL", "students")));
        }

        internal static void ThrowLicense(SMSLicenseInfo lic)
        {
            if (string.IsNullOrEmpty(lic.StatusReason))
                throw new SMSException(SR2.GetString("LIIV"));
            else
                throw new SMSException(lic.StatusReason);
        }

        internal static void ThrowLicenseActivation()
        {
            throw new SMSException(SR2.GetString("EOWLA"));
            //Error occurred while license activation.
        }

        internal static void ThrowLicenseChecking()
        {
            throw new SMSException(SR2.GetString("EOWLC"));
            //Error occurred while license checking.
        }

        internal static void ThrowLicenseIsNull()
        {
            throw new SMSException(SR2.GetString("LINOE"));
            //License is null or empty.
        }

        internal static void ThrowLicenseIsInvalid()
        {
            throw new SMSException(SR2.GetString("LIIV"));
            //License is invalid.
        }

        internal static void ThrowNoLicense()
        {
            throw new SMSException(SR2.GetString("LINFPRL"));
            //License is not found. Please register license.
        }

        internal static void ThrowLicenseKeyIsNull()
        {
            throw new SMSException(SR2.GetString("LKINOE"));
            //License key is null or empty.
        }

        internal static void ThrowLicenseKeyIsIncorrect()
        {
            throw new SMSException(SR2.GetString("LKIIC"));
            //License key is incorrect.
        }

        internal static void ThrowLicenseKeyIsInvalid()
        {
            throw new SMSException(SR2.GetString("LKIIV"));
            //License key is invalid.
        }

        internal static void ThrowLicenseKeyIsInvalid(Exception inner)
        {
            throw new SMSException(SR2.GetString("LKIIV"), inner);
            //License key is invalid.
        }

        internal static void ThrowSMS(string message)
        {
            throw new SMSException(message);
        }

        internal static void ThrowSMS(string message, Exception inner)
        {
            throw new SMSException(message, inner);
        }

        internal static void ThrowRNF()
        {
            throw new SMSException(SR.GetString("RNF"));
        }

        internal static void ThrowIDL()
        {
            throw new SMSException(SR.GetString("IDL"));
        }

        // sms.license
        private static string GetL()
        {
            return new string(LText.Select(m => ConvL(m, 0.5, 65)).ToArray());
        }

        private static char ConvL(ulong x, double factor, byte mask)
        {
            return (char)Math.Pow((x ^ mask), factor);
        }

        #endregion
    }
}
