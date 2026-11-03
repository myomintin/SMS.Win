using CodeflexSystems.SmartUI.Presentation;
using CodeflexSystems.SmartUI.Windows;
using CodeflexSystems.SMS.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CodeflexSystems.SMS.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public sealed partial class ShellView : ModernWindow
    {
        #region Fields

        private bool canLogon;
        private Link logonMenu;
        private Uri cacheContentSource;

        #endregion

        #region Properties

        #endregion

        public ShellView()
        {
            InitializeComponent();
            this.logonMenu = this.TitleLinks.FirstOrDefault();
            this.cacheContentSource = null;
            if (this.canLogon = this.logonMenu != null)
            {
                this.MenuLinkGroups.Clear();
                this.cacheContentSource = this.ContentSource;
                this.ContentSource = null;
            }
            this.Title = ApplicationManager.Current.ApplicationName;
            DoFakeLogon("Administrator");
            //DoFakeLogon("Account");
        }

        public void Logon()
        {
            if (!this.canLogon)
                return;

            bool flag = true;
            try
            {
#if DEBUG
                if (ApplicationManager.Current.IsAuthenticated)
                {
                    ApplicationManager.Current.Logoff();
                    this.logonMenu.DisplayName = "Logon";
                    this.MenuLinkGroups.Clear();
                    this.ClearHistoryAndContentCache();
                    this.ContentSource = null;
                }
                else
                {
                    var view = NavigationManager.ShowLogon();

                    if (view.DialogResult == true)
                    {
                        App.CursorWait();
                        if (ApplicationManager.Current.IsFakeLogon)
                            ApplicationManager.Current.FakeLogon(view.ViewModel.UserName);
                        else
                            ApplicationManager.Current.Logon(view.ViewModel.UserName, view.ViewModel.Password);

                        if (ApplicationManager.Current.IsAuthenticated)
                        {
                            var loader = this.ContentLoader as SMSContentLoader;

                            if (loader != null)
                                loader.Reset();
                            this.logonMenu.DisplayName = string.Format("Logout {0}",
                                   ApplicationManager.Current.User.Identity.Name);
                            if (ApplicationManager.Current.IsFakeLogon)
                                ApplicationManager.Current.BuildApplicationMenus(this.MenuLinkGroups,
                                    ApplicationManager.Current.GetFakeUserPermissions());
                            else
                                ApplicationManager.Current.BuildApplicationMenus(this.MenuLinkGroups,
                                    ApplicationManager.Current.GetUserPermissions());
                            this.ClearHistoryAndContentCache();
                            SetContentSource();
                        }
                        else
                            DialogManager.ShowError("Logon", "Invalid user name and password.");
                        App.CursorDefault();
                    }
                }
#else
                if (ApplicationManager.Current.IsAuthenticated)
                {
                    flag = false;
                    ApplicationManager.Current.Logoff();
                    this.logonMenu.DisplayName = "Logon";
                    this.MenuLinkGroups.Clear();
                    this.ClearHistoryAndContentCache();
                    this.ContentSource = null;
                }
                else
                {
                    var view = NavigationManager.ShowLogon();

                    if (view.DialogResult == true)
                    {
                        App.CursorWait();
                        ApplicationManager.Current.Logon(view.ViewModel.UserName, view.ViewModel.Password);
                        if (ApplicationManager.Current.IsAuthenticated)
                        {
                            var loader = this.ContentLoader as SMSContentLoader;

                            flag = false;
                            if (loader != null)
                                loader.Reset();
                            this.logonMenu.DisplayName = string.Format("Logout {0}",
                                   ApplicationManager.Current.User.Identity.Name);
                            ApplicationManager.Current.BuildApplicationMenus(this.MenuLinkGroups,
                                ApplicationManager.Current.GetUserPermissions());
                            this.ClearHistoryAndContentCache();
                            SetContentSource();
                        }
                        else
                            DialogManager.ShowError("Logon", "Invalid user name and password.");
                        App.CursorDefault();
                    }
                    else
                        flag = false;
                }
#endif
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Logon", ex);
            }
            if (flag)
                Logon();
        }

        [Conditional("DEBUG")]
        private void DoFakeLogon(string userName)
        {
            try
            {
                ApplicationManager.Current.FakeLogon(userName);
                if (ApplicationManager.Current.IsAuthenticated)
                {
                    var loader = this.ContentLoader as SMSContentLoader;

                    if (loader != null)
                        loader.Reset();
                    this.logonMenu.DisplayName = string.Format("Logout {0}",
                           ApplicationManager.Current.User.Identity.Name);
                    ApplicationManager.Current.BuildApplicationMenus(this.MenuLinkGroups,
                        ApplicationManager.Current.GetFakeUserPermissions());
                    this.ClearHistoryAndContentCache();
                    SetContentSource();
                }
                else
                    DialogManager.ShowError("Logon", "Invalid user name and password.");
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Logon", ex);
            }
        }

        private void SetContentSource()
        {
            bool flag = ApplicationManager.Current.IsFakeLogon && this.cacheContentSource != null;

            if (flag)
                this.ContentSource = this.cacheContentSource;
            else
            {
                var gm = this.MenuLinkGroups.FirstOrDefault();

                if (gm != null)
                {
                    var lm = gm.Links.FirstOrDefault();

                    if (lm != null)
                        this.ContentSource = lm.Source;
                }
            }
        }
    }
}
