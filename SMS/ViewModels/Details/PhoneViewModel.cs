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
    internal sealed class PhoneViewModel : EntityViewModelCM<Phone>
    {
        #region Properties

        public ObservableCollection<PhoneType> PhoneTypes { get; private set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Number is required.")]
        [MaxLength(24, ErrorMessage = "Phone Number is too long.")]
        [MinLength(5, ErrorMessage = "Phone Number must be at least 5 digits.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Phone Number.")]
        public string PhoneNumber
        {
            get { return this.Model.PhoneNumber; }
            set
            {
                if (this.Model.PhoneNumber != value)
                {
                    ValidateProperty(value, "PhoneNumber");
                    this.Model.PhoneNumber = value;
                    RaiseDataChanged("PhoneNumber");
                }
            }
        }

        [MaxLength(10, ErrorMessage = "Extension is too long.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Invalid Extension.")]
        public string Extension
        {
            get { return this.Model.Extension; }
            set
            {
                Repair(ref value);
                if (this.Model.Extension != value)
                {
                    ValidateProperty(value, "Extension");
                    this.Model.Extension = value;
                    RaiseDataChanged("Extension");
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
            get { return IsDummy(this.Model.PhoneId); }
        }

        public PhoneType PhoneType
        {
            get { return this.Model.PhoneType; }
            set
            {
                if (this.Model.PhoneType != value)
                {
                    this.Model.PhoneType = value;
                    RaiseDataChanged("PhoneType");
                }
            }
        }

        #endregion
        
        #region Ctor, Dtor

        public PhoneViewModel(Phone model)
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
            if (string.IsNullOrEmpty(this.PhoneNumber))
                ValidateProperty(this.PhoneNumber, "PhoneNumber");
        }

        public void LoadData(IParent parent)
        {
            if (this.IsLoaded)
                return;

            this.Parent = parent;
            SetCreatedBy();
            SetModifiedBy();
            this.PhoneTypes = this.Parent.UoW.PhoneTypes;
            if (this.PhoneType == null)
                this.Model.PhoneType = this.PhoneTypes.GetDefault();

            this.DataChanged += (s, e) =>
            {
                DetectErrors();
            };
            this.IsLoaded = true;
        }

        #endregion
    }
}
