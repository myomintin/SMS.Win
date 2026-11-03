using CodeflexSystems.Library.Core;
using CodeflexSystems.Library.Extensions;
using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class UserViewModel : EntityViewModelCM<User>
    {
        #region Properties

        public EntityViewModelCollection<UserPermissionViewModel, UserPermission> UserPermissionViewModels { get; private set; }
        public bool CanSavePermissions { get; internal set; }
        public bool IsDetectedChanges { get; private set; }

        public IUserParent UserParent
        {
            get
            {
                return this.Parent as IUserParent;
            }
        }

        [Required(AllowEmptyStrings = false, ErrorMessage = "User Name is required.")]
        [MaxLength(128, ErrorMessage = "User Name is too long.")]
        [MinLength(5, ErrorMessage = "User Name is too short.")]
        [RegularExpression(@"^[0-9A-Za-z]+$", ErrorMessage = "Special characters are not allowed.")]
        public string UserName
        {
            get { return this.Model.UserName; }
            set
            {
                if (this.Model.UserName != value)
                {
                    ValidateProperty(value, "UserName");
                    this.Model.UserName = value;
                    RaiseDataChanged("UserName");
                }
            }
        }

        [MaxLength(128, ErrorMessage = "Full Name is too long.")]
        public string FullName
        {
            get { return this.Model.FullName; }
            set
            {
                Repair(ref value);
                if (this.Model.FullName != value)
                {
                    ValidateProperty(value, "FullName");
                    this.Model.FullName = value;
                    RaiseDataChanged("FullName");
                }
            }
        }

        public DateTime? BirthDate
        {
            get { return this.Model.BirthDate; }
            set
            {
                if (this.Model.BirthDate != value)
                {
                    this.Model.BirthDate = value;
                    RaiseDataChanged("BirthDate");
                }
            }
        }

        [MaxLength(128, ErrorMessage = "Email Address is too long.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string EmailAddress
        {
            get { return this.Model.EmailAddress; }
            set
            {
                Repair(ref value);
                if (this.Model.EmailAddress != value)
                {
                    ValidateProperty(value, "EmailAddress");
                    this.Model.EmailAddress = value;
                    RaiseDataChanged("EmailAddress");
                }
            }
        }

        [MaxLength(250, ErrorMessage = "Description is too long.")]
        public string Description
        {
            get { return this.Model.Description; }
            set
            {
                Repair(ref value);
                if (this.Model.Description != value)
                {
                    ValidateProperty(value, "Description");
                    this.Model.Description = value;
                    RaiseDataChanged("Description");
                }
            }
        }

        public DateTime CreatedDate
        {
            get { return this.Model.CreatedDate; }
        }

        public DateTime ModifiedDate
        {
            get { return this.Model.ModifiedDate; }
        }

        public DateTime CreatedDateLT
        {
            get { return this.Model.CreatedDate.ToLocalTime(); }
        }

        public DateTime ModifiedDateLT
        {
            get { return this.Model.ModifiedDate.ToLocalTime(); }
        }

        public override bool CanEdit
        {
            get
            {
                if (this.IsNew)
                    return false;

                return base.CanEdit;
            }
        }

        public override bool CanDelete
        {
            get
            {
                if (this.IsLoaded || this.IsNew || this.DetectCanDelete == null)
                    return this.Model.CanDelete;

                return this.DetectCanDelete();
            }
        }

        public override bool IsNew
        {
            get { return IsDummy(this.Model.UserId); }
        }

        public bool IsLogonUser
        {
            get { return ApplicationManager.Current.IsLogonUser(this.Model.UserId); }
        }

        public bool Disabled
        {
            get { return this.Model.UserMembership.Disabled; }
            set
            {
                if (this.Model.UserMembership.Disabled != value)
                {
                    this.Model.UserMembership.Disabled = value;
                    RaiseDataChanged("Disabled");
                }
            }
        }

        public DateTime? DisabledDate
        {
            get { return this.Model.UserMembership.DisabledDate; }
        }

        public DateTime? DisabledDateLT
        {
            get { return this.Model.UserMembership.DisabledDate.ToLocalTime(); }
        }

        public DateTime? LogonDate
        {
            get { return this.Model.UserMembership.LogonDate; }
        }

        public DateTime? LogonDateLT
        {
            get { return this.Model.UserMembership.LogonDate.ToLocalTime(); }
        }

        public DateTime? PasswordModifiedDate
        {
            get { return this.Model.UserMembership.PasswordModifiedDate; }
        }

        public DateTime? PasswordModifiedDateLT
        {
            get { return this.Model.UserMembership.PasswordModifiedDate.ToLocalTime(); }
        }

        public byte PasswordFailures
        {
            get { return this.Model.UserMembership.PasswordFailures; }
        }

        public DateTime? PasswordFailureDate
        {
            get { return this.Model.UserMembership.PasswordFailureDate; }
        }

        public DateTime? PasswordFailureDateLT
        {
            get { return this.Model.UserMembership.PasswordFailureDate.ToLocalTime(); }
        }

        public DateTime? PasswordAnswerModifiedDate
        {
            get { return this.Model.UserMembership.PasswordAnswerModifiedDate; }
        }

        public DateTime? PasswordAnswerModifiedDateLT
        {
            get { return this.Model.UserMembership.PasswordAnswerModifiedDate.ToLocalTime(); }
        }

        public byte PasswordAnswerFailures
        {
            get { return this.Model.UserMembership.PasswordAnswerFailures; }
        }

        public DateTime? PasswordAnswerFailureDate
        {
            get { return this.Model.UserMembership.PasswordAnswerFailureDate; }
        }

        public DateTime? PasswordAnswerFailureDateLT
        {
            get { return this.Model.UserMembership.PasswordAnswerFailureDate.ToLocalTime(); }
        }

        #endregion

        #region Ctor, Dtor

        public UserViewModel(User model, IUserParent parent)
            : base(model, parent)
        {
            if (this.Model.UserMembership == null)
                this.Model.UserMembership = new UserMembership();
        }

        #endregion

        #region Methods

        public override void AcceptChanges()
        {
            if (!this.IsLoaded)
                return;

            this.UserPermissionViewModels.AcceptAllChanges();
            this.CanSavePermissions = false;
            this.IsDetectedChanges = false;
            SetCreatedBy();
            SetModifiedBy();
            BaseAcceptChanges();
        }

        public override void Refresh()
        {
            if (!this.IsLoaded || this.OldObjectState == ObjectState.Added || this.Parent == null)
                return;

            bool removeAdded = this.Parent.RemoveAdded;

            this.Parent.UoW.RefreshToOriginalValues(this.Model);
            this.Parent.UoW.RefreshToOriginalValues(this.Model.UserMembership);
            this.Parent.UoW.RefreshRangeToOriginalValues(this.Model.UserPermissions, removeAdded);
            this.UserPermissionViewModels.Refresh(removeAdded);
            InitialFixUserPermissions();
            this.CanSavePermissions = false;
            this.IsDetectedChanges = false;
            BaseRefresh();
        }

        internal void DetectChanges()
        {
            if (this.IsDetectedChanges || this.Parent == null)
                return;

            if (this.ObjectState == ObjectState.Deleted && this.OldObjectState != ObjectState.Added)
            {
                this.Parent.UoW.RemoveRange(this.Model.UserPermissions);
                this.Parent.UoW.Remove(this.Model.UserMembership);
                this.Parent.UoW.Remove(this.Model.UserOption);
                this.Parent.UoW.Remove(this.Model);
            }
            else if (this.OverallState == ObjectState.Added || this.OverallState == ObjectState.Modified)
            {
                FinalFixUserPermissions();
                this.UserPermissionViewModels.Where(vm =>
                    vm.ObjectState == ObjectState.Deleted && vm.OldObjectState != ObjectState.Added)
                    .ForEach(vm => this.Parent.UoW.Remove(vm.Model));
                this.UserPermissionViewModels.Where(vm => vm.ObjectState == ObjectState.Added)
                    .ForEach(vm => this.Model.UserPermissions.AddIfNotExist(vm.Model));

                if (this.ObjectState == ObjectState.Added)
                {
                    this.Parent.UoW.SetMemebership(this.Model.UserMembership, this.Model.UserMembership.Password, null, null);
                    this.Parent.UoW.Add(this.Model);
                }
            }
            this.IsDetectedChanges = true;
        }

        protected override void SetCreatedBy()
        {
            if (string.IsNullOrEmpty(this.CreatedBy))
                this.CreatedBy = this.Parent.UoW.GetUserNameById(this.Model.CreatedBy);
        }

        protected override void SetModifiedBy()
        {
            this.ModifiedBy = this.Parent.UoW.GetUserNameById(this.Model.ModifiedBy);
        }

        public void DetectErrors()
        {
            if (!this.IsLoaded)
                return;

            ValidateRequiredProperties();
            this.CanSave = !this.HasAnyErrors || this.CanSavePermissions;
        }

        public void ValidateRequiredProperties()
        {
            if (string.IsNullOrEmpty(this.UserName))
                ValidateProperty(this.UserName, "UserName");
        }

        public void LoadData()
        {
            if (this.IsLoaded)
                return;

            SetCreatedBy();
            SetModifiedBy();
            this.UserPermissionViewModels = new EntityViewModelCollection<UserPermissionViewModel, UserPermission>();
            this.UserPermissionViewModels.AddRange(this.Model.UserPermissions
                .To(m => new UserPermissionViewModel(m)), false);
            InitialFixUserPermissions();
            this.DataChanged += (s, e) =>
            {
                DetectErrors();
            };
            this.IsLoaded = true;
        }

        public bool ResetPassword(string newPassword)
        {
            bool flag = this.Parent.UoW.ResetPassword(this.Model, newPassword);

            if (flag)
                RaiseRelatedDataChanged(null);

            return flag;
        }

        public bool IsCanView(Guid id)
        {
            var vm = this.UserPermissionViewModels.FirstOrDefault(m => m.MenuId == id);

            if (vm != null)
                return vm.IsAllowed(Permission.CanView);

            return false;
        }

        public void SetCanView(Guid id, bool flag)
        {
            var vm = this.UserPermissionViewModels.FirstOrDefault(m => m.MenuId == id);

            if (vm != null)
            {
                Permission v = Permission.CanView;
                if (vm.IsAllowed(v) != flag)
                {
                    vm.SetPermission(v, flag);
                    this.CanSavePermissions = true;
                    RaiseRelatedDataChanged(null);
                }
            }
        }

        public UserPermissionViewModel GetUserPermissionViewModel(Guid id)
        {
            var vm = this.UserPermissionViewModels.FirstOrDefault(m => m.MenuId == id);

            if (vm != null)
                vm.LoadData(this.Parent);

            return vm;
        }

        public void AddUserPermission(Guid id)
        {
            if (this.UserPermissionViewModels.Any(vm => vm.MenuId == id) == false)
            {
                var vm = new UserPermissionViewModel(new UserPermission { MenuId = id });

                this.UserPermissionViewModels.Add(vm);
                //RaiseRelatedDataChanged(null);
            }
        }

        private void InitialFixUserPermissions()
        {
            ApplicationManager.Current.GroupMenus.ForEach(gm => gm.Value2.ForEach(lm =>
            {
                AddUserPermission(lm.Key);
            }));
        }

        private void FinalFixUserPermissions()
        {
            var gkeys = ApplicationManager.Current.GroupMenus
                .SelectMany(gm => gm.Value2.Select(lm => lm.Key)).ToList();
            var pkeys = this.UserPermissionViewModels.Select(up => up.MenuId).ToList();

            pkeys.ForEach(p =>
            {
                if (gkeys.Any(g => g == p) == false)
                {
                    var vm = this.UserPermissionViewModels.FirstOrDefault(m => m.MenuId == p);

                    if (vm != null)
                        this.UserPermissionViewModels.Delete(vm);
                }
            });
        }

        #endregion
    }
}
