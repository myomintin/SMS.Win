using CodeflexSystems.Library.Core;
using CodeflexSystems.SMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class UserResetPasswordViewModel : BaseViewModel
    {
        #region Fields

        private string password;
        private string confirmPassword;

        #endregion

        #region Properties

        public string Password
        {
            get { return this.password; }
            set
            {
                if (this.password != value)
                {
                    this.password = value;
                    RaisePropertyChanged("Password");
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

        public UserResetPasswordViewModel()
        {
            Clear();
        }

        #endregion

        #region Methods

        private void ValidatePasswords()
        {
            bool flag = this.password == this.confirmPassword;

            RemoveErrors("Password");
            if (string.IsNullOrEmpty(this.password) == false &&
                string.IsNullOrEmpty(this.confirmPassword) == false &&
                flag == false)
                SetErrors("Password", "Password and Confirm Password must be same.");
            else
            {
                try
                {
                    ApplicationManager.Current.ValidatePassword(this.password);
                }
                catch (Exception ex)
                {
                    SetErrors("Password", ex.Message);
                }
            }

            if (this.HasErrors)
                DialogManager.ShowError("Error", this.GetErrorString(string.Empty));
            this.CanSave = !this.HasErrors && flag;
        }

        public void Clear()
        {
            this.password = string.Empty;
            this.confirmPassword = string.Empty;
            this.CanSave = false;
            RemoveErrors("Password");
        }

        #endregion
    }
}
