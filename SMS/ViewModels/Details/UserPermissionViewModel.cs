using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class UserPermissionViewModel : EntityViewModelCM<UserPermission>
    {
        #region Properties

        public Guid MenuId
        {
            get { return this.Model.MenuId; }
            set
            {
                if (this.Model.MenuId != value)
                {
                    this.Model.MenuId = value;
                    RaiseDataChanged("MenuId");
                }
            }
        }

        public Permission Permission
        {
            get { return this.Model.Permission; }
            set
            {
                if (this.Model.Permission != value)
                {
                    this.Model.Permission = value;
                    RaiseDataChanged("Permission");
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

        #endregion

        #region Ctor, Dtor

        public UserPermissionViewModel(UserPermission model)
            : base(model)
        { }

        #endregion

        #region Methods

        protected override void SetCreatedBy()
        {
            if (string.IsNullOrEmpty(this.CreatedBy))
                this.CreatedBy = this.Parent.UoW.GetUserNameById(this.Model.CreatedBy);
        }

        protected override void SetModifiedBy()
        {
            this.ModifiedBy = this.Parent.UoW.GetUserNameById(this.Model.ModifiedBy);
        }

        public void LoadData(IParent parent)
        {
            if (this.IsLoaded)
                return;

            this.Parent = parent;
            SetCreatedBy();
            SetModifiedBy();
            this.IsLoaded = true;
        }

        public bool IsAllowed(Permission value)
        {
            if (value == Permission.None)
                return this.Model.Permission == value;

            return (this.Model.Permission & value) == value;
        }

        public void SetPermission(Permission value, bool flag)
        {
            if (flag)
            {
                if (value == Permission.None)
                    this.Permission = Permission.None;
                else
                    this.Permission = this.Model.Permission | value;
            }
            else
                this.Permission = this.Model.Permission & ~value;
        }

        #endregion
    }
}
