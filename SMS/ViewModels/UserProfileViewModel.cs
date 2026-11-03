using CodeflexSystems.Library.Core;
using CodeflexSystems.SmartUI.Windows.Input;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Data;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class UserProfileViewModel : BaseViewModel
    {
        #region Fields

        private string fullName;
        private string description;
        private User model;

        #endregion

        #region Properties

        public ISMSUnitOfWork UoW { get; private set; }
        public UserChangePasswordViewModel ChangePasswordViewModel { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand ChangePasswordCommand { get; private set; }
        public string UserName { get; private set; }
        public bool IsFoundUser { get; private set; }

        [MaxLength(128, ErrorMessage = "Full Name is too long.")]
        public string FullName
        {
            get { return this.fullName; }
            set
            {
                Repair(ref value);
                if (this.fullName != value)
                {
                    ValidateProperty(value, "FullName");
                    this.fullName = value;
                    RaisePropertyChanged("FullName");
                }
            }
        }

        [MaxLength(250, ErrorMessage = "Description is too long.")]
        public string Description
        {
            get { return this.description; }
            set
            {
                Repair(ref value);
                if (this.description != value)
                {
                    ValidateProperty(value, "Description");
                    this.description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public UserProfileViewModel()
        {
            this.ViewName = "User Profile";
            LoadData();
        }

        #endregion

        #region Methods

        internal void ClearAllCollections()
        {
            this.UoW.Clear();
        }

        private void LoadData()
        {
            var userid = ApplicationManager.Current.UserId;
            if (this.IsLoaded || !userid.HasValue)
                return;

            this.UoW = ApplicationManager.Resolve<ISMSUnitOfWork>();
            var id = userid.Value;
            this.model = this.UoW.FirstOrDefault<User>(m => m.UserId == id);

            if (this.IsFoundUser = this.model != null)
            {
                this.UserName = this.model.UserName;
                this.fullName = this.model.FullName;
                this.description = this.model.Description;
            }
            this.ChangePasswordViewModel = new UserChangePasswordViewModel();
            this.ChangePasswordViewModel.ViewName = string.Format("Change password for {0}", this.UserName);
            this.PropertyChanged += (s, e) =>
            {
                this.CanSave = this.IsFoundUser && !this.HasErrors;
            };
            this.SaveCommand = new RelayCommand(o => OnSave(), o => this.CanSave);
            this.ChangePasswordCommand = new RelayCommand(o => OnChangePassword(), o => this.IsFoundUser);
            this.IsLoaded = true;
        }

        public void OnSave()
        {
            if (!this.IsLoaded || !this.CanSave || !this.IsFoundUser)
                return;

            try
            {
                if (this.model != null)
                {
                    this.model.FullName = this.fullName;
                    this.model.Description = this.description;
                    this.UoW.SaveChanges();
                }
                this.CanSave = false;
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Save Profile", ex);
            }
        }

        private void OnChangePassword()
        {
            if (!this.IsLoaded || !this.IsFoundUser)
                return;

            try
            {
                if (NavigationManager.ShowUserChangePassword(this.ChangePasswordViewModel).DialogResult == true)
                {
                    bool flag = false;

                    flag = this.UoW.ChangePassword(this.UserName, this.ChangePasswordViewModel.CurrentPassword,
                        this.ChangePasswordViewModel.NewPassword, true);
                    if (flag)
                        DialogManager.Show("Change Password", "Changed your password successfully.", null);
                    else
                        DialogManager.ShowError("Change Password", "Invalid current password.");
                }
            }
            catch (Exception ex)
            {
                DialogManager.ShowErrorAndLog("Change Password", ex);
            }
        }

        #endregion
    }
}
