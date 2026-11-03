using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class UserSearchListViewModel : SearchListOptionViewModelBDSDI<UserViewModel, User, UserSearchOption>,
        IUserParent
    {
        #region Fields

        private bool editing = false;

        #endregion

        #region Properties

        public UserResetPasswordViewModel UserResetPasswordViewModel { get; private set; }

        #endregion

        #region Ctor, Dtor

        public UserSearchListViewModel()
        {
            Initialize();
            InitializeCommands();
        }

        #endregion

        #region Methods

        protected override void OnDispose()
        {
            base.OnDispose();
            DisposeUoW();
        }

        private void DisposeUoW()
        {
            if (this.UoW != null)
                this.UoW.Dispose();
        }

        internal void ClearAllCollections()
        {
            Clear();
            this.UoW.Clear();
        }

        private void Initialize()
        {
            try
            {
                this.IsLoaded = false;
                this.SearchOptionInfos.AddRange(SearchOptionHelper.GetUserSearchOptions()
                    .To(m => SearchOptionHelper.GetSearchInfo(m)), false);
                this.SearchOptionInfos.SelectedIndex = 1;
                this.MinIntegral = 0;
                this.MaxIntegral = 255;

                this.UserResetPasswordViewModel = new UserResetPasswordViewModel();
                this.ConvertTo = (m) => new UserViewModel(m, this);
                this.RemoveAdded = true;
                this.IsLoaded = true;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize", ex);
            }
        }

        protected override void InitializeCommands()
        {
            try
            {
                this.SearchCommand = new RelayCommand(o => OnSearch(), o => this.CanSearch);
                this.EditCommand = new RelayCommand(o => OnEdit(), o => this.Collection.CanEdit);
                this.DeleteCommand = new RelayCommand(o => OnPermissions(o), o => this.Collection.CanEdit);
                this.SaveCommand = new RelayCommand(o => OnResetPassword(o), o => this.Collection.CanEdit);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize Commands", ex);
            }
        }

        protected override void OnSearch()
        {
            try
            {
                App.CursorWait();
                var current = this.SearchOptionInfos.SelectedItem;
                IQueryable<User> query = null;

                if (current.SearchValueType == SearchValueType.Boolean)
                {
                    query = GetBooleanQuery<UserMembership>().Select(m => m.User);
                }
                else if (current.SearchValueType == SearchValueType.DateTime)
                {
                    if (current.Option == UserSearchOption.CreatedDate || current.Option == UserSearchOption.ModifiedDate)
                    {
                        query = GetDateQuery<User>();
                    }
                    else
                    {
                        query = GetDateQuery<UserMembership>().Select(m => m.User);
                    }
                }
                else if (current.SearchValueType == SearchValueType.Text)
                {
                    if (current.Option == UserSearchOption.CreatedBy)
                    {
                        query = GetStringQuery<User, User, Guid?>(m => m.CreatedBy == null,
                            "UserName", m => m.CreatedBy, u => u.UserId, (m, u) => m);
                    }
                    else if (current.Option == UserSearchOption.ModifiedBy)
                    {
                        query = GetStringQuery<User, User, Guid?>(m => m.ModifiedBy == null,
                            "UserName", m => m.ModifiedBy, u => u.UserId, (m, u) => m);
                    }
                    else
                    {
                        query = GetStringQuery<User>();
                    }
                }
                else if (current.SearchValueType == SearchValueType.Integral)
                {
                    query = GetIntegralQuery<UserMembership>().Select(m => m.User);
                }

                this.Collection.Clear();
                if (query != null)
                    this.Collection.AddRange(query.Ordered().To(m => this.ConvertTo(m)), false);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog(this.ViewName, ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        public override void Edit(UserViewModel vm)
        {
            vm.DetectChanges();
            this.UoW.SaveChanges();
            vm.AcceptChanges();
            OnRefreshBinding();
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                this.editing = true;
                if (NavigationManager.ShowUserEdit(vm).DialogResult == true)
                    Edit(vm);
                else
                {
                    if (vm.HasAnyErrors)
                    {
                        DialogResult result = DialogManager.AskConfirmation("Confirmation",
                            SR.GetString("EditDataAsk3", "user"),
                            SR.GetString("EditDataAsk4"), DialogButton.YesNo);
                        if (result == DialogResult.Yes)
                            flag = true;
                        else
                        {
                            OnEdit();
                            return;
                        }
                    }
                    else if (vm.CanSave)
                        flag = true;
                }

                if (flag)
                {
                    vm.Refresh();
                    OnRefreshBinding();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Edit Data", ex);
            }
            finally
            {
                this.editing = false;
            }
        }

        private void OnPermissions(object o)
        {
            try
            {
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowUserPermissions(vm, o).DialogResult == true)
                {
                    if (this.editing == false)
                    {
                        Edit(vm);
                        DialogManager.Show("Changed Permissions", "Changed Permissions successfully.");
                    }
                    //if (vm.IsNotLogonUser == false)
                    //    DialogManager.ShowInfo("Changed Permissions",
                    //        string.Format("Changed the current logged on user permissions{0}will be effected the next time logon.", Environment.NewLine));
                }
                else if (this.editing == false && vm.CanSavePermissions
                    && vm.OldObjectState != ObjectState.Added)
                {
                    vm.Refresh();
                    OnRefreshBinding();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Permissions", ex);
            }
        }

        private void OnResetPassword(object o)
        {
            try
            {
                var vm = this.Collection.SelectedItem;

                Setup(vm);
                this.UserResetPasswordViewModel.ViewName = string.Format("Reset password for {0}", vm.UserName);
                if (NavigationManager.ShowUserResetPassword(this.UserResetPasswordViewModel, o).DialogResult == true)
                {
                    vm.ResetPassword(this.UserResetPasswordViewModel.Password);
                    if (!this.editing)
                    {
                        Edit(vm);
                        DialogManager.Show("Reset Password", "Reset password successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Reset Password", ex);
            }
            finally
            {
                this.UserResetPasswordViewModel.Clear();
            }
        }

        internal override void OnSave()
        { }

        private void Setup(UserViewModel vm)
        {
            if (vm == null)
                return;

            vm.SetViewName("User");
            if (vm.IsLoaded)
                return;

            vm.DataChanged += (s, e) =>
            {
                if (e.PropertyName == "UserName")
                {
                    bool flag = false;

                    foreach (var item in this.Collection.Where(m =>
                        string.Equals(vm.UserName, m.UserName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (flag = (item != vm))
                            break;
                    }
                    if (flag == false)
                    {
                        var value = this.UoW.FirstOrDefaultAsNoTracking<User>(m =>
                            string.Equals(vm.UserName, m.UserName, StringComparison.InvariantCultureIgnoreCase));

                        if (value != null && value.UserId != vm.Model.UserId)
                            flag = true;
                    }
                    if (flag)
                    {
                        vm.SetErrors(e.PropertyName, SR.GetString("AlreadyExists1", "User Name", vm.UserName));
                        vm.DetectErrors();
                    }
                }
            };
            vm.LoadData();
        }

        #endregion
    }
}
