using CodeflexSystems.SMS.Core;
using CodeflexSystems.SMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.ViewModels
{
    internal sealed class CountryViewModel : EntityViewModelCM<Country>
    {
        #region Properties

        [Required(AllowEmptyStrings = false, ErrorMessage = "Country Name is required.")]
        [MaxLength(128, ErrorMessage = "Country Name is too long.")]
        public string CountryName
        {
            get { return this.Model.CountryName; }
            set
            {
                if (this.Model.CountryName != value)
                {
                    ValidateProperty(value, "CountryName");
                    this.Model.CountryName = value;
                    RaiseDataChanged("CountryName");
                }
            }
        }

        [MaxLength(3, ErrorMessage = "Country Code is too long.")]
        public string CountryCode
        {
            get { return this.Model.CountryCode; }
            set
            {
                Repair(ref value);
                if (this.Model.CountryCode != value)
                {
                    ValidateProperty(value, "CountryCode");
                    this.Model.CountryCode = value;
                    RaiseDataChanged("CountryCode");
                }
            }
        }

        [MaxLength(5, ErrorMessage = "Code is too long.")]
        public string Code
        {
            get { return this.Model.Code; }
            set
            {
                Repair(ref value);
                if (this.Model.Code != value)
                {
                    ValidateProperty(value, "Code");
                    this.Model.Code = value;
                    RaiseDataChanged("Code");
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
            get { return IsDummy(this.Model.CountryId); }
        }

        public bool IsReadOnly { get; set; }

        #endregion

        #region Ctor, Dtor

        public CountryViewModel(Country model)
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
            if (string.IsNullOrEmpty(this.CountryName))
                ValidateProperty(this.CountryName, "CountryName");
        }

        public void LoadData(IParent parent)
        {
            if (this.IsLoaded)
                return;

            this.Parent = parent;
            SetCreatedBy();
            SetModifiedBy();
            this.DataChanged += (s, e) =>
            {
                DetectErrors();
            };
            this.IsLoaded = true;
        }

        #endregion
    }
}
