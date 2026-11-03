using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Extensions;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class UserListViewModel : ListViewModelPaging<UserViewModel, User>, IUserParent
    {
        #region Fields

        private bool editing = false;

        #endregion

        #region Properties

        public UserResetPasswordViewModel UserResetPasswordViewModel { get; private set; }

        #endregion

        #region Ctor, Dtor

        public UserListViewModel()
        {
            Initialize();
            LoadData();
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
                this.UserResetPasswordViewModel = new UserResetPasswordViewModel();
                this.ConvertTo = (m) => new UserViewModel(m, this);
                this.Paging.GetTotalCount = () => this.UoW.Count<User>();
                this.Paging.GetData = () =>
                {
                    return this.UoW.SelectAll<User>().OrderedList()
                        .Skip(this.Paging.SkippedIndex).Take(this.Paging.PageSize)
                        .To(m => this.ConvertTo(m));
                };
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
                this.AddCommand = new RelayCommand(o => OnAdd(), o => this.CanAdd);
                this.EditCommand = new RelayCommand(o => OnEdit(), o => this.Paging.CanEdit);
                this.DeleteCommand = new RelayCommand(o => OnPermissions(o), o => this.Paging.CanEdit);
                this.SaveCommand = new RelayCommand(o => OnResetPassword(o), o => this.Paging.CanEdit);
                this.RefreshCommand = new RelayCommand(o => OnRefresh(), o => this.CanRefresh);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Initialize Commands", ex);
            }
        }

        private void LoadData()
        {
            try
            {
                this.CanAdd = this.CanRefresh = true;
                if (!this.Refreshing)
                    this.Paging.Page = 1;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Load Data", ex);
            }
        }

        public override void Add(UserViewModel vm)
        {
            vm.Model.UserMembership.Password = this.UserResetPasswordViewModel.Password;
            vm.DetectChanges();
            this.UoW.SaveChanges();
            vm.AcceptChanges();
            this.Paging.Add(vm, true);
        }

        protected override void OnAdd()
        {
            try
            {
                var vm = new UserViewModel(new User(), this);

                Setup(vm);
                this.editing = true;
                if (NavigationManager.ShowUserNew(vm).DialogResult == true)
                    Add(vm);
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Add User", ex);
            }
            finally
            {
                this.editing = false;
                this.UserResetPasswordViewModel.Clear();
            }
        }

        public override void Edit(UserViewModel vm)
        {
            vm.DetectChanges();
            this.UoW.SaveChanges();
            vm.AcceptChanges();
            //OnRefreshBinding();
        }

        protected override void OnEdit()
        {
            try
            {
                bool flag = false;
                var vm = this.Paging.SelectedItem;

                Setup(vm);
                this.editing = true;
                if (NavigationManager.ShowUserEdit(vm).DialogResult == true)
                {
                    Edit(vm);
                    DialogManager.Show("Edit User", "the user is edited successfully.");
                }
                else
                {
                    if (vm.OldObjectState == ObjectState.Added)
                    {
                        if (vm.HasAnyErrors)
                        {
                            DialogResult result = DialogManager.AskConfirmation("Confirmation",
                                SR.GetString("EditDataAsk1", "user"),
                                SR.GetString("EditDataAsk2"), DialogButton.YesNo);
                            if (result == DialogResult.Yes)
                                this.Paging.Remove(vm);
                            else
                            {
                                OnEdit();
                                return;
                            }
                        }
                    }
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
                }

                if (flag)
                {
                    vm.Refresh();
                    OnRefreshBinding();
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Edit User", ex);
            }
            finally
            {
                this.editing = false;
            }
        }

        protected override void OnRefresh()
        {
            try
            {
                if (this.CanSave)
                {
                    DialogResult result = DialogManager.AskConfirmation("Confirmation",
                        SR.GetString("RefreshDataAsk1"), SR.GetString("RefreshDataAsk2"), DialogButton.OKCancel);
                    if (result == DialogResult.Cancel)
                        return;
                }

                int page = this.Paging.Page;
                int index = this.Paging.SelectedIndexOfPage;

                App.CursorWait();
                ClearAllCollections();
                DisposeUoW();
                this.UoW = ApplicationManager.Resolve<ISMSUnitOfWork>();

                this.Refreshing = true;
                LoadData();
                this.Paging.RefreshPage(page);
                this.Paging.SelectedIndex = index;
                this.Refreshing = false;
                //OnRefreshBinding();
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Refresh User", ex);
            }
            finally
            {
                App.CursorDefault();
            }
        }

        private void OnPermissions(object o)
        {
            try
            {
                var vm = this.Paging.SelectedItem;

                Setup(vm);
                if (NavigationManager.ShowUserPermissions(vm, o).DialogResult == true)
                {
                    if (!this.editing)
                    {
                        Edit(vm);
                        DialogManager.Show("Changed Permissions", "Changed permissions successfully.");
                    }
                    //if (!vm.IsNotLogonUser)
                    //    DialogManager.ShowInfo("Changed Permissions",
                    //        string.Format("Changed the current logged on user permissions{0}will be effected the next time logon.", Environment.NewLine));
                }
                else if (!this.editing && vm.CanSavePermissions
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
                var vm = this.Paging.SelectedItem;

                Setup(vm);
                this.UserResetPasswordViewModel.ViewName = string.Format("Reset password for {0}", vm.UserName);
                if (NavigationManager.ShowUserResetPassword(this.UserResetPasswordViewModel, o).DialogResult == true)
                {
                    vm.ResetPassword(this.UserResetPasswordViewModel.Password);
                    if (this.editing == false)
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

                    foreach (var item in this.Paging.Where(m =>
                        string.Equals(vm.UserName, m.UserName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        if (flag = (item != vm))
                            break;
                    }
                    if (!flag)
                    {
                        var value = this.UoW.FirstOrDefaultAsNoTracking<User>(m =>
                            vm.UserName.Equals(m.UserName, StringComparison.InvariantCultureIgnoreCase));

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
