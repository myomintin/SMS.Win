using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Extensions;
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
    internal class EmailViewModel : EntityViewModelCM<Email>
    {
        #region Properties

        public ObservableCollection<EmailType> EmailTypes { get; private set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Address is required.")]
        [MaxLength(128, ErrorMessage = "Email Address is too long.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string EmailAddress
        {
            get { return this.Model.EmailAddress; }
            set
            {
                if (this.Model.EmailAddress != value)
                {
                    ValidateProperty(value, "EmailAddress");
                    this.Model.EmailAddress = value;
                    RaiseDataChanged("EmailAddress");
                }
            }
        }

        public bool IsDefault
        {
            get { return this.Model.IsDefault; }
            set
            {
                if (this.Model.IsDefault != value)
                {
                    this.Model.IsDefault = value;
                    RaiseDataChanged("IsDefault");
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
            get { return this.Model.CanDelete; }
        }

        public override bool IsNew
        {
            get { return IsDummy(this.Model.EmailId); }
        }

        public EmailType EmailType
        {
            get { return this.Model.EmailType; }
            set
            {
                if (this.Model.EmailType != value)
                {
                    this.Model.EmailType = value;
                    RaiseDataChanged("EmailType");
                }
            }
        }

        #endregion

        #region Ctor, Dtor

        public EmailViewModel(Email model)
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

        public void DetectErrors()
        {
            if (!this.IsLoaded)
                return;

            ValidateRequiredProperties();
            this.CanSave = !this.HasAnyErrors;
        }

        public void ValidateRequiredProperties()
        {
            if (string.IsNullOrEmpty(this.EmailAddress))
                ValidateProperty(this.EmailAddress, "EmailAddress");
        }

        public void LoadData(IParent parent)
        {
            if (this.IsLoaded)
                return;

            this.Parent = parent;
            SetCreatedBy();
            SetModifiedBy();
            this.EmailTypes = this.Parent.UoW.EmailTypes;
            if (this.EmailType == null)
                this.Model.EmailType = this.EmailTypes.GetDefault();

            this.DataChanged += (s, e) =>
            {
                DetectErrors();
            };
            this.IsLoaded = true;
        }

        #endregion
    }
}
