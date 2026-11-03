using CodeflexSystems.Library.Windows;
using CodeflexSystems.SMS.Controls;
using CodeflexSystems.SMS.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CodeflexSystems.SMS.Core
{
    #region Enums

    public enum OpenStyle
    {
        None,
        Window,
        Dialog
    }

    #endregion

    internal static class NavigationManager
    {
        public static Window ShowAcademicYear(object viewmodel)
        {
            var window = ApplicationManager.Resolve<AcademicYearEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowAdd(object viewmodel, object control)
        {
            var window = ApplicationManager.Resolve<AddView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        public static Window ShowAddressType(object viewmodel)
        {
            var window = ApplicationManager.Resolve<AddressTypeEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static BusyPopupView ShowBusyPopup()
        {
            var window = ApplicationManager.Resolve<BusyPopupView>();
            window.Owner = App.Current.MainWindow;
            ShowWindow(window, OpenStyle.Window);

            return window;
        }

        public static Window ShowClass(object viewmodel)
        {
            var window = ApplicationManager.Resolve<ClassEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowAddCountries(object viewmodel)
        {
            var window = ApplicationManager.Resolve<AddCountriesView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowCountry(object viewmodel)
        {
            var window = ApplicationManager.Resolve<CountryEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowCourse(object viewmodel)
        {
            var window = ApplicationManager.Resolve<CourseEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowAddCurrencies(object viewmodel)
        {
            var window = ApplicationManager.Resolve<AddCurrenciesView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowCurrency(object viewmodel)
        {
            var window = ApplicationManager.Resolve<CurrencyEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowDueDatePopup(object viewmodel)
        {
            var window = ApplicationManager.Resolve<DueDatePopupView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowEmail(object viewmodel, object control)
        {
            var window = ApplicationManager.Resolve<EmailEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        public static Window ShowEmailType(object viewmodel)
        {
            var window = ApplicationManager.Resolve<EmailTypeEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowEnlargeImage(object viewmodel, ImageViewer control)
        {
            var window = ApplicationManager.Resolve<EnlargeImageView>();
            window.Height = control.EnlargeHeight;
            window.Width = control.EnlargeWidth;
            window.ResizeMode = control.EnlargeResizeMode;
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        public static Window ShowExtendedAddress(object viewmodel)
        {
            var window = ApplicationManager.Resolve<ExtendedAddressEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowExtendedGuardian(object viewmodel)
        {
            var window = ApplicationManager.Resolve<ExtendedGuardianEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowFeeType(object viewmodel)
        {
            var window = ApplicationManager.Resolve<FeeTypeEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowGuardianType(object viewmodel)
        {
            var window = ApplicationManager.Resolve<GuardianTypeEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static LRCSView ShowLRCS()
        {
            var window = ApplicationManager.Resolve<LRCSView>();
            ShowWindow(window, OpenStyle.Dialog);

            return window;
        }

        public static LRSOView ShowLRSO()
        {
            var window = ApplicationManager.Resolve<LRSOView>();
            ShowWindow(window, OpenStyle.Dialog);

            return window;
        }

        public static LRSTView ShowLRST()
        {
            var window = ApplicationManager.Resolve<LRSTView>();
            ShowWindow(window, OpenStyle.Dialog);

            return window;
        }

        public static LRSTHView ShowLRSTH()
        {
            var window = ApplicationManager.Resolve<LRSTHView>();
            ShowWindow(window, OpenStyle.Dialog);

            return window;
        }

        public static LogonView ShowLogon()
        {
            var window = ApplicationManager.Resolve<LogonView>();
            ShowWindow(window, OpenStyle.Dialog);

            return window;
        }

        public static Window ShowMainWindow(OpenStyle style = OpenStyle.None)
        {
            var window = ApplicationManager.Resolve<ShellView>();

            return ShowWindow(window, style);
        }

        public static Window ShowPersonAddress(object viewmodel, object control)
        {
            var window = ApplicationManager.Resolve<PersonAddressEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        public static Window ShowPersonGuardian(object viewmodel, object control)
        {
            var window = ApplicationManager.Resolve<PersonGuardianEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        public static Window ShowPhone(object viewmodel, object control)
        {
            var window = ApplicationManager.Resolve<PhoneEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        public static Window ShowPhoneType(object viewmodel)
        {
            var window = ApplicationManager.Resolve<PhoneTypeEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowReceipt(object viewmodel)
        {
            var window = ApplicationManager.Resolve<ReceiptEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowReceiptPopup(object viewmodel)
        {
            var window = ApplicationManager.Resolve<ReceiptPopupView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowReceiptDetail(object viewmodel, object control)
        {
            var window = ApplicationManager.Resolve<ReceiptDetailEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        public static Window ShowReligion(object viewmodel)
        {
            var window = ApplicationManager.Resolve<ReligionEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowReminderPopup(object viewmodel)
        {
            var window = ApplicationManager.Resolve<ReminderPopupView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowReportViewer(object viewmodel)
        {
            var window = ApplicationManager.Resolve<ReportViewerView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Window);
        }

        public static Window ShowSchool(object viewmodel)
        {
            var window = ApplicationManager.Resolve<SchoolEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowSearchAddress(object viewmodel, object control)
        {
            var window = ApplicationManager.Resolve<SearchAddressView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        public static Window ShowSearchGuardian(object viewmodel, object control)
        {
            var window = ApplicationManager.Resolve<SearchGuardianView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        public static Window ShowSearchSectionInfo(object viewmodel, object control)
        {
            var window = ApplicationManager.Resolve<SearchSectionInfoView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        public static Window ShowSearchStudent(object viewmodel, object control)
        {
            var window = ApplicationManager.Resolve<SearchStudentView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        public static Window ShowSearchStudentSectionInfo(object viewmodel)
        {
            var window = ApplicationManager.Resolve<SearchStudentSectionInfoView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowSearchStudentSectionInfoPlus(object viewmodel)
        {
            var window = ApplicationManager.Resolve<SearchStudentSectionInfoPlusView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowSection(object viewmodel)
        {
            var window = ApplicationManager.Resolve<SectionEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowStudent(object viewmodel)
        {
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            //Console.WriteLine("Show Student - Create : Start");
            var window = ApplicationManager.Resolve<StudentEditView>();
            window.DataContext = viewmodel;

            //sw.Stop();
            //Console.WriteLine("Show Student - Create : End : {0}", sw.Elapsed);
            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowStudentDiscount(object viewmodel)
        {
            var window = ApplicationManager.Resolve<StudentDiscountEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowStudentEducation(object viewmodel, object control)
        {
            var window = ApplicationManager.Resolve<StudentEducationEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        public static Window ShowStudentFerryFee(object viewmodel)
        {
            var window = ApplicationManager.Resolve<StudentFerryFeeEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowStudentInfoPopup(object viewmodel)
        {
            var window = ApplicationManager.Resolve<StudentInfoPopupView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowUserChangePassword(object viewmodel)
        {
            var window = ApplicationManager.Resolve<UserChangePasswordView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowUserEdit(object viewmodel)
        {
            var window = ApplicationManager.Resolve<UserEditView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowUserNew(object viewmodel)
        {
            var window = ApplicationManager.Resolve<UserNewView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog);
        }

        public static Window ShowUserPermissions(object viewmodel, object control)
        {
            var window = ApplicationManager.Resolve<UserPermissionsView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        public static Window ShowUserResetPassword(object viewmodel, object control)
        {
            var window = ApplicationManager.Resolve<UserResetPasswordView>();
            window.DataContext = viewmodel;

            return ShowWindow(window, OpenStyle.Dialog, control);
        }

        private static Window ShowWindow(Window window, OpenStyle style, object parent = null)
        {
            if (style == OpenStyle.Window)
                window.Show();
            else if (style == OpenStyle.Dialog)
            {
                window.SetOwner(parent);
                //window.HasOwner(hasOwnerLocation: WindowStartupLocation.CenterScreen);
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.ShowDialog();
            }

            return window;
        }
    }
}
