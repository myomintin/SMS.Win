using CodeflexSystems.Library.Core;
using CodeflexSystems.SMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class UserChangePasswordViewModel : BaseViewModel
    {
        #region Fields

        private string currentPassword;
        private string newPassword;
        private string confirmPassword;

        #endregion

        #region Properties

        public string CurrentPassword
        {
            get { return this.currentPassword; }
            set
            {
                if (this.currentPassword != value)
                {
                    this.currentPassword = value;
                    RaisePropertyChanged("CurrentPassword");
                }
            }
        }

        public string NewPassword
        {
            get { return this.newPassword; }
            set
            {
                if (this.newPassword != value)
                {
                    this.newPassword = value;
                    RaisePropertyChanged("NewPassword");
                    ValidatePasswords();
                }
            }
        }

        public string ConfirmPassword
        {
            get { return this.confirmPassword; }
            set
            {
                if (this.confirmPassword != value)
                {
                    this.confirmPassword = value;
                    RaisePropertyChanged("ConfirmPassword");
                    ValidatePasswords();
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public UserChangePasswordViewModel()
        {
            Clear();
        }

        #endregion

        #region Methods

        private void ValidatePasswords()
        {
            bool flag = this.newPassword == this.confirmPassword;

            RemoveErrors("NewPassword");
            if (string.IsNullOrEmpty(this.newPassword) == false &&
                string.IsNullOrEmpty(this.confirmPassword) == false &&
                flag == false)
                SetErrors("NewPassword", "New Password and Confirm Password must be same.");
            else
            {
                try
                {
                    ApplicationManager.Current.ValidatePassword(this.newPassword);
                }
                catch (Exception ex)
                {
                    SetErrors("NewPassword", ex.Message);
                }
            }

            if (this.HasErrors)
                DialogManager.ShowError("Error", this.GetErrorString(string.Empty));
            this.CanSave = !this.HasErrors && flag && !string.IsNullOrEmpty(this.currentPassword);
        }

        public void Clear()
        {
            this.currentPassword = string.Empty;
            this.newPassword = string.Empty;
            this.confirmPassword = string.Empty;
            this.CanSave = false;
            RemoveErrors("NewPassword");
        }

        #endregion
    }
}
